@echo off
echo Starting WasteNaut MVP...
echo.
echo Starting backend server...
start "WasteNaut Backend" cmd /k "dotnet run --project backend/api/aspnet/WasteNaut.Admin/WasteNaut.Admin.csproj"
echo.
echo Waiting for server to start...
timeout /t 5 /nobreak >nul
echo.
echo Opening frontend in your default browser...
start "" "http://localhost:3000"
echo.
echo MVP is now running! You can also:
echo - Navigate to other pages using the menu
echo - Test the organization dashboard
echo - Try the responsive design on mobile
echo - Access API docs at: http://localhost:3000/swagger
echo.
echo Press any key to close this window...
pause >nul
