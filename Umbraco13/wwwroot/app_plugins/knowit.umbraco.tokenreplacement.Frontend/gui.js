//we're not gonna bother trying to angular our way through this one

// detect when the url changes and re-initialize the gui when it does
let wlocation = window.location.href;
function urlChange() {
    if (wlocation != window.location.href) {
        wlocation = window.location.href;
        eventListeners = {};

        if(!wlocation.includes('umbraco#/content')) {
            document.querySelectorAll(".token-box").forEach(x => x.remove())
            return;
        }

        invoke();
        
    }
    setTimeout(urlChange, 100);
}
urlChange();

let invokeTimeout;

// detect when the dom changes and re-initialize the gui when it does
const callback = (mutationsList, observer) => {
    for (let mutation of mutationsList) {
        if (mutation.type === 'childList') {
            if(mutation.target.getAttribute("ng-show") == "infiniteMode") {
                document.querySelectorAll(".infiniteModeTokenBox").forEach(x => x.remove()) 
            }
            // Check each added node to see if it matches or contains the target
            mutation.addedNodes.forEach(node => {
                if (node.nodeType === Node.ELEMENT_NODE) {
                    if (mutation.target.classList.contains("umb-split-views") || 
                        mutation.target.classList.contains("controls") ||
                        mutation.target.classList.contains("alert-success") 
                    ) {
                        
                        clearTimeout(invokeTimeout);
                        // Set a new timeout to call invoke() after 500 ms
                        invokeTimeout = setTimeout(() => {
                            invoke();
                        }, 500);
                    }
                }
            });
        }
    }
};

// Create an observer instance linked to the callback function
const observer = new MutationObserver(callback);

// Options for the observer (which mutations to observe)
const config = {
    childList: true,
    subtree: true
};


// Start observing the entire body to catch when the target element is added
observer.observe(document.body, config);

let timeout;
let content;
let eventListeners = {}
let dictionary = {};

function invoke() {
    document.body.querySelectorAll('.umb-sub-views-nav-item__action').forEach(button => {
        button.addEventListener('click', () => {
            invoke();
        });
    }, { once: true });

    // fetch all tokens and initialize the gui
    fetch('/umbraco/api/tokenreplacement/tokens').then(response => response.json()).then(data => {
        dictionary = data;
        // inject a datalist element with all the tokens (for autocomplete functionality)
        createDatalist(data, document)

        debouncedGui();
        // re-initialize the gui when the window is resized
        window.addEventListener('resize', () => setTimeout(() => debouncedGui(), 250));
    });
}

function createDatalist(data, root) {
    // Create datalist element
    document.querySelector("#tokenReplacementDatalist")?.remove();
    const datalist = root.createElement('datalist');
    datalist.id = 'tokenReplacementDatalist';

    // Iterate through each key in the data object
    for (const key in data) {
        // Create an option element
        const option = document.createElement('option');
        option.value = data[key][0].key;  // Set the key as the value
        option.label = data[key][0].value;  // Set the value of the first item as the label
        datalist.appendChild(option);
    }

    // Append the datalist to the body or wherever it needs to go
    root.body.appendChild(datalist);
}

function debouncedGui() {

    // remove all drawn tokens from screen
    document.querySelectorAll(".token-box").forEach(x => x.remove())

    // for each content pane (we could be in split-view)
    document.querySelectorAll('.umb-pane-content').forEach(content => {

        // detect culture
        const culture = content.querySelector('.umb-property-variant-label')?.innerText.trim();

        // handle iframes
        content.querySelectorAll('.tox-edit-area iframe')?.forEach(rte => {
            handleIframe(rte, culture);
        });
        // handle text areas
        content.querySelectorAll('.umb-textarea')?.forEach(element => {
            handleElement(element, culture);
        });

        // handle text strings
        content.querySelectorAll('.umb-textstring')?.forEach(element => {
            handleElement(element, culture);
        });

    });
    
    // detect in-frame scrolling in the editor and redraw the gui if scrolled.
    let timeout;
    document.querySelector(".umb-editor-container")?.addEventListener('scroll', function () {
        document.querySelectorAll(".token-box").forEach(x => x.remove())
        clearTimeout(timeout);
        timeout = setTimeout(debouncedGui, 100);
    });
    
}

function handleIframe(element, culture) {
    // make iframe relative so positioning of drawn elements gets easier
    element.parentNode.style.position = 'relative';

    // get the iframe's body (ie. the RTE)
    const iframe = element.contentWindow.document.body;

    
    // first perform cleanup in case we've already processed the iframe
    // remove all the custom elements we've inserted, so we start from scratch
    let text = iframe.innerHTML;
    iframe.querySelectorAll('.token-replacement-iframe-match').forEach(span => {
        const t = span.innerText;
        iframe.innerHTML = iframe.innerHTML.replace(span.outerHTML, t);
    });

    // now match {{any.key}} in the innerHTML of the iframe
    const regex = /{{[a-zA-Z0-9 .]+}}/g;
    const matches = text.match(regex);

    // for each match, replace it with a span element that contains the match and a magic button
    matches?.forEach((match, index) => {
        const key = match.replace('{{', '').replace('}}', '');
        const value = dictionary[key] ? dictionary[key][0].value : 'Token not found';

        const isError = value == 'Token not found';
        const extraClass = isError ? 'error' : '';

        iframe.innerHTML = element.contentWindow.document.body.innerHTML.replaceAll(match,
            '<span class="token-replacement-iframe-match ' + extraClass + '" title="' + value + '">' + match + '<button style="display:none"></button></span>'
        );

    });
    // make sure we only set the event listener once by locking it to the culture and id
    if (!eventListeners[culture + element.id]) {
        // this madness inserts a {{token}} whenever the user has pressed { and then another {
        eventListeners[culture + element.id] = function (e) {
            if (e.key != "Control" && e.key != "AltGraph" && e.key != "Backspace") {
                if (lastKey == "{" && e.key == "{") {
                    setTimeout(() => {
                        e.target.querySelectorAll('p').forEach(p => {
                            if (p.innerText.startsWith("{{") && !p.innerText.includes("}}")) {
                                p.innerText = "{{token}}"
                                debouncedGui();
                            }
                            if (p.innerText.endsWith("{{") && !p.innerText.includes("}}")) {
                                p.innerText = "{{token}}"
                                debouncedGui();
                            }
                        });
                    }, 100);

                }
                lastKey = e.key;
            }
            else lastKey = null;
        }
        // attach to keydown (can't use keyup because it'll detect 7 instead of {)
        iframe.addEventListener("keydown", eventListeners[culture + element.id]);
    }
    // style the spans and buttons and attach event listeners
    setTimeout(() => {
        element.contentWindow.document.querySelectorAll('.token-replacement-iframe-match').forEach(span => {
            
            const error = span.classList.contains('error');
            const btn = span.querySelector('button');
            
            span.style.position = 'relative';
            
            btn.style.display = 'block';
            btn.style.position = 'absolute';
            btn.style.left = 0;
            btn.style.top = "20px"
            btn.style.width = "100%";
            btn.style.border = 'none';
            btn.style.height = "3px"
            btn.style.cursor = 'pointer';
            btn.style.backgroundColor = error ? 'red' : 'lightblue';
            btn.style.userSelect = "none";
            
            btn.addEventListener('mouseenter', () => {
                btn.style.backgroundColor = error ? 'lightcoral' : 'lightcoral'; // Change color on hover
            });
            btn.addEventListener('mouseleave', () => {
                btn.style.backgroundColor = error ? 'red' : 'lightblue'; // Restore original color
            });

            const btnRect = btn.getBoundingClientRect();

            const key = span.innerText.replace('{{', '').replace('}}', '');

            // inject an input field to write in when the button is clicked
            // the input field is added to the original document and not the iframe
            btn.addEventListener('click', () => {
                const inputField = document.createElement('input');
                if (!error)
                    inputField.value = key;
                // Style the input field with absolute positioning
                inputField.style.position = 'absolute';
                inputField.style.border = "1px solid black";
                inputField.style.background = "white";
                inputField.style.top = `${btnRect.top}px`; // Position just below boxDiv
                inputField.style.left = `${btnRect.left}px`; // Align with the left edge of boxDiv
                inputField.style.zIndex = 29; // Ensure it's on top of everything else
                inputField.setAttribute("list", "tokenReplacementDatalist");

                // Append the input field to the parent of boxDiv
                element.parentNode.appendChild(inputField);

                // Optionally, focus on the input field so that user can start typing immediately
                inputField.focus();
                inputField.addEventListener('focusout', () => {
                    try {
                        inputField.remove();
                    }
                    catch { }
                });

                // when a change happens, update the text inside the iframe
                inputField.addEventListener('change', () => {
                    span.innerText = span.innerText.replace('{{' + key + '}}', '{{' + inputField.value + '}}');
                    // rerun iframe logic
                    handleIframe(element, culture)
                });
            });
        });
    }, 20);
}

let lastKey = null;
function handleElement(element, culture) {
    // make sure we only set the event listener once by locking it to the culture and id
    if (!eventListeners[culture + element.id]) {
        // this madness inserts a {{token}} whenever the user has pressed { and then another {
        eventListeners[culture + element.id] = function (e) {
            if (e.key != "Control" && e.key != "AltGraph" && e.key != "Backspace") {
                if (lastKey == "{" && e.key == "{") {
                    const oldVal = element.value.split("");
                    setTimeout(() => {
                        const newVal = element.value.split("");
                        let index = element.selectionEnd;
                        oldVal.forEach((v, i) => {
                            if (v != newVal[i] && index == -1) {
                                index = i;
                            }
                        });
                        const sub = element.value.toString().substring(0, index) + "token}}" + element.value.toString().substring(index);
                        element.value = sub;
                        element.selectionEnd = index + 7;
                        debouncedGui();
                    }, 100);
                }
                
                lastKey = e.key;
            }
            else lastKey = null;
            debouncedGui();
        };
        element.addEventListener('keydown', eventListeners[culture + element.id]);
    }
    manageTokens(element, culture + element.id);
}
function indexOfOccurrence(haystack, needle, occurrence) {

    var counter = 0;
    var index = -1;
    do {
        index = haystack.indexOf(needle, index + 1);
    }
    while (index !== -1 && (++counter < occurrence));
    return index;
}
function manageTokens(element, id) {
    document.body.querySelectorAll('.token-box[data-id="' + id + "boxDiv" + '"]').forEach(box => { box.remove() });
    let text = element.value;
    const regex = /{{[a-zA-Z0-9 .]+}}/g;
    const matches = text.match(regex);

    const matchCount = {};

    matches?.forEach((match, index) => {

        if (matchCount[match] === undefined) {
            matchCount[match] = 1;
        }
        else matchCount[match] = matchCount[match] + 1;
        // Find the starting index of 'match' in 'text' from the 'index'
        const startIndexOfMatch = indexOfOccurrence(text, match, matchCount[match])//text.indexOf(match, index);

        // Get the substring from the start of 'text' to the start index of 'match'
        const sub = text.substring(0, startIndexOfMatch);

        // Split 'sub' into lines to determine the line number of the match
        const lines = sub.split('\n');

        // The line number where the match is found
        const lineNumber = lines.length;

        // Extract the full line of text that contains the match
        // by splitting the original text into lines and using the line number
        const fullTextLines = text.split('\n');
        let lineWithMatch = fullTextLines[lineNumber - 1];  // Adjust by -1 because line numbers are 1-based
        lineWithMatch = lineWithMatch.substring(0, lineWithMatch.indexOf(match) + match.length);

        const startRect = measureText(sub + match, element);
        const measure = measureText(match, element);
        const width = measureText(lineWithMatch, element);
        const boxPosition = {
            top: startRect.bottom - measure.height + measure.height,
            left: startRect.left + (width.width - measure.width) + 6,
            width: measure.width - 4,
            height: 3
        };
        BuildMenuBox(boxPosition, match, element, id);
    });


    element.value = text;
}

// for everything NOT RTE
// build a box under the text that is clickable and makes an input field appear
function BuildMenuBox(boxPosition, key, element, id) {
    key = key.replace('{{', '').replace('}}', '')

    const boxDiv = document.createElement('div');
    boxDiv.classList.add('token-box');
    boxDiv.style.position = 'absolute';
    boxDiv.style.top = `${boxPosition.top}px`;
    boxDiv.style.left = `${boxPosition.left}px`;
    boxDiv.style.width = `${boxPosition.width}px`;
    boxDiv.style.height = `${boxPosition.height}px`;
    const hasInfiniteModeParent = element.closest('div[ng-show="infiniteMode"]') !== null;

    boxDiv.style.zIndex = hasInfiniteModeParent ? 7501 : 29;
    if(hasInfiniteModeParent) 
        boxDiv.classList.add('infiniteModeTokenBox');
    boxDiv.setAttribute('data-id', id + "boxDiv");
    boxDiv.title = dictionary[key] ? dictionary[key][0].value : 'Token not found';

    const isError = dictionary[key] === undefined;
    if (isError)
        boxDiv.classList.add('error')

    document.body.appendChild(boxDiv);

    // Check if boxDiv has a parent with div[ng-show="infiniteMode"]

    boxDiv.addEventListener('click', (e) => {
        // Create the input field
        const inputField = document.createElement('input');
        if (!isError)
            inputField.value = key;
        // Style the input field with absolute positioning
        inputField.style.position = 'absolute';
        inputField.style.top = `${boxDiv.offsetTop + boxDiv.offsetHeight}px`; // Position just below boxDiv
        inputField.style.left = `${boxDiv.offsetLeft}px`; // Align with the left edge of boxDiv
        inputField.style.zIndex = 9999; // Ensure it's on top of everything else
        inputField.setAttribute("list", "tokenReplacementDatalist");

        // Append the input field to the parent of boxDiv
        boxDiv.parentNode.appendChild(inputField);

        // focus and make sure the input field is removed when it loses focus
        inputField.focus();
        inputField.addEventListener('focusout', () => {
            try {
                inputField.remove();
            }
            catch { }
        });
        // when a change happens, update the text inside the iframe
        inputField.addEventListener('change', () => {
            element.value = element.value.replace('{{' + key + '}}', '{{' + inputField.value + '}}');
            element.dispatchEvent(new Event('change'));
            manageTokens(element)

        });
    });

    return boxDiv;
}

// try not to think about this one...
// if you insist, it's a function that measures the text of an element
// you can't get the "position" of a piece of text inside a textarea, so we have to 
// create a div that is styled the same way as the textarea and then measure the text inside that div
function measureText(text, element) {
    // Create a div that covers getBoundingClientRect of element
    const tempDiv = document.createElement('div');
    tempDiv.style.position = 'absolute';
    tempDiv.style.top = `${element.getBoundingClientRect().top}px`;
    tempDiv.style.left = `${element.getBoundingClientRect().left}px`;
    tempDiv.style.maxWidth = `${element.getBoundingClientRect().width}px`;
    tempDiv.style.backgroundColor = 'blue';
    tempDiv.style.zIndex = 9999;
    tempDiv.style.paddingLeft = '6px'
    tempDiv.style.paddingTop = '4px'
    tempDiv.style.whiteSpace = 'pre-wrap';
    tempDiv.style.visibility = 'hidden';
    
    var computedStyle = window.getComputedStyle(element);

    // Apply font styles to blueDiv
    tempDiv.style.fontStyle = computedStyle.fontStyle;
    tempDiv.style.fontWeight = computedStyle.fontWeight;
    tempDiv.style.fontSize = computedStyle.fontSize;
    tempDiv.style.fontFamily = computedStyle.fontFamily;
    tempDiv.style.lineHeight = computedStyle.lineHeight;
    tempDiv.style.textTransform = computedStyle.textTransform;
    tempDiv.style.textDecoration = computedStyle.textDecoration;
    tempDiv.style.letterSpacing = computedStyle.letterSpacing;
    tempDiv.style.textShadow = computedStyle.textShadow;
    tempDiv.innerHTML = text;

    document.body.appendChild(tempDiv);

    const rect = tempDiv.getBoundingClientRect()

    document.body.removeChild(tempDiv);

    return rect;
}
