@echo off
echo Starting Alpha Cinema...
echo.
echo Installing root dependencies...
call npm install
echo.
echo Installing frontend dependencies...
cd frontend
call npm install
cd ..
echo.
echo Running both Backend and Frontend...
call npm run dev
pause
