# Bachelor-Project-MyBranch

Setup Instructions

Backend Setup

1. Navigate to the Backend folder using your terminal or command line.
2. Install the necessary dependencies by running the command: (only on setup of project) dotnet restore.
3. Start the backend server by running the command: dotnet run.
4. Verify that the backend is running on the following URLs: HTTP: http://localhost:5030 HTTPS: https://localhost:7063

Frontend Setup

1. Navigate to the frontend folder using your terminal or command line.  
2.  Install the necessary dependencies by running the command: (only on setup of project) npm install.
3. Start the frontend server by running the command: npm start.
4. Open your browser and navigate to http://localhost:3000 to view the application.

Debugging in VS Code If you want to debug both the .NET backend and the React frontend at the same time, we have a compound debug configuration set up in .vscode/launch.json. To use it:

1.  Open the project’s root folder in VS Code (the folder that contains both Backend/ and frontend/).
2.  Click the “Run and Debug” icon in the sidebar
3.  At the top of the Run and Debug panel, change the debug configuration to “Launch All.”
4.  Press F5 (or click “Start Debugging”).
5.  VS Code will build and launch the .NET backend (on ports specified in your launchSettings.json) and run npm start for the React frontend at http://localhost:3000.
6.  You can then place breakpoints in C# backend code (within Backend/) and JavaScript/TypeScript frontend code (within frontend/) to debug both parts of the application simultaneously.
