/**
 * Simple Chatbot Demo
 * A lightweight chatbot widget with keyword-based responses
 */

(function() {
    'use strict';

    // Chatbot configuration
    const config = {
        botName: 'Assistant',
        welcomeMessage: 'Hello! How can I help you today?',
        primaryColor: '#2c3e50',
        position: 'right'
    };

    // Simple response database
    const responses = {
        greetings: ['hello', 'hi', 'hey', 'greetings', 'howdy'],
        goodbye: ['bye', 'goodbye', 'see you', 'farewell'],
        thanks: ['thank', 'thanks', 'appreciate'],
        help: ['help', 'assist', 'support', 'need help'],
        funds: ['fund', 'investment', 'portfolio', 'returns'],
        contact: ['contact', 'email', 'phone', 'reach'],
        about: ['about', 'who are', 'what do', 'company'],
        services: ['service', 'offer', 'provide', 'do you do'],
        default: ["I'm not sure I understand. Could you rephrase that?", "I'm still learning! Try asking about our funds, services, or how to contact us."]
    };

    const responseMessages = {
        greetings: "Hello! Welcome to our platform. How can I assist you today?",
        goodbye: "Goodbye! Feel free to come back if you have more questions.",
        thanks: "You're welcome! Is there anything else I can help with?",
        help: "I can help you with information about our funds, services, and how to contact us. What would you like to know?",
        funds: "We offer a variety of investment funds with different risk profiles. You can view our fund performance table on this page. Would you like more specific information?",
        contact: "You can reach us through the contact form below, or email us at info@example.com. We typically respond within 24 hours.",
        about: "We are a leading investment management firm dedicated to helping clients achieve their financial goals through strategic fund management.",
        services: "We offer investment management, portfolio analysis, financial planning, and advisory services. Check out our portfolio section for more details!"
    };

    // DOM elements
    let chatWidget, chatButton, chatWindow, chatMessages, chatInput, sendButton;

    // Initialize chatbot
    function init() {
        if (document.getElementById('chatbot-widget')) return; // Already initialized

        createWidget();
        addEventListeners();
        addMessage(config.botName, config.welcomeMessage, 'bot');
    }

    // Create widget HTML
    function createWidget() {
        const widgetHTML = `
            <div id="chatbot-widget">
                <button id="chatbot-toggle" class="chatbot-toggle" aria-label="Toggle chat">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                        <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
                    </svg>
                    <span class="chatbot-close-icon" style="display: none;">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <line x1="18" y1="6" x2="6" y2="18"></line>
                            <line x1="6" y1="6" x2="18" y2="18"></line>
                        </svg>
                    </span>
                </button>
                <div id="chatbot-window" class="chatbot-window">
                    <div class="chatbot-header">
                        <span class="chatbot-title">${config.botName}</span>
                        <button class="chatbot-minimize" aria-label="Minimize chat">
                            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                <line x1="18" y1="6" x2="6" y2="18"></line>
                                <line x1="6" y1="6" x2="18" y2="18"></line>
                            </svg>
                        </button>
                    </div>
                    <div id="chatbot-messages" class="chatbot-messages"></div>
                    <div class="chatbot-input-area">
                        <input type="text" id="chatbot-input" class="chatbot-input" placeholder="Type a message..." aria-label="Chat input">
                        <button id="chatbot-send" class="chatbot-send" aria-label="Send message">
                            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                <line x1="22" y1="2" x2="11" y2="13"></line>
                                <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', widgetHTML);

        chatWidget = document.getElementById('chatbot-widget');
        chatButton = document.getElementById('chatbot-toggle');
        chatWindow = document.getElementById('chatbot-window');
        chatMessages = document.getElementById('chatbot-messages');
        chatInput = document.getElementById('chatbot-input');
        sendButton = document.getElementById('chatbot-send');
    }

    // Add event listeners
    function addEventListeners() {
        chatButton.addEventListener('click', toggleChat);
        chatWindow.querySelector('.chatbot-minimize').addEventListener('click', toggleChat);
        sendButton.addEventListener('click', sendMessage);
        chatInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') sendMessage();
        });
    }

    // Toggle chat window
    function toggleChat() {
        const isOpen = chatWindow.classList.contains('open');
        chatWindow.classList.toggle('open', !isOpen);
        chatButton.classList.toggle('active', !isOpen);

        const openIcon = chatButton.querySelector('svg:first-child');
        const closeIcon = chatButton.querySelector('.chatbot-close-icon');
        openIcon.style.display = isOpen ? 'block' : 'none';
        closeIcon.style.display = isOpen ? 'none' : 'block';

        if (!isOpen) {
            chatInput.focus();
        }
    }

    // Send message
    function sendMessage() {
        const message = chatInput.value.trim();
        if (!message) return;

        addMessage('You', message, 'user');
        chatInput.value = '';

        // Simulate typing delay
        setTimeout(() => {
            const response = getResponse(message);
            addMessage(config.botName, response, 'bot');
        }, 500 + Math.random() * 500);
    }

    // Get response based on message content
    function getResponse(message) {
        const lowerMessage = message.toLowerCase();

        for (const [category, keywords] of Object.entries(responses)) {
            if (category === 'default') continue;
            if (keywords.some(keyword => lowerMessage.includes(keyword))) {
                return responseMessages[category];
            }
        }

        return responseMessages.default[Math.floor(Math.random() * responseMessages.default.length)];
    }

    // Add message to chat
    function addMessage(sender, message, type) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `chatbot-message chatbot-message-${type}`;
        messageDiv.innerHTML = `
            <span class="chatbot-message-sender">${sender}:</span>
            <span class="chatbot-message-text">${escapeHtml(message)}</span>
        `;
        chatMessages.appendChild(messageDiv);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    // Escape HTML to prevent XSS
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Expose toggle function globally for manual control
    window.Chatbot = {
        toggle: () => chatButton ? chatButton.click() : null,
        isOpen: () => chatWindow ? chatWindow.classList.contains('open') : false
    };

})();
