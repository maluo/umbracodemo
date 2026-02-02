@echo off
echo Installing Python dependencies...
python -m pip install -r requirements.txt
if %errorlevel% neq 0 (
    echo.
    echo Error: Failed to install dependencies. 
    echo Please make sure Python is installed and added to your PATH.
    pause
    exit /b %errorlevel%
)
echo.
echo Dependencies installed successfully.
echo.
echo Note: Ensure Tesseract OCR is installed from:
echo https://github.com/UB-Mannheim/tesseract/wiki
echo.
pause
