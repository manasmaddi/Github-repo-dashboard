# 🚀 GitHub Repository Dashboard

A **Blazor Server** application built on **.NET** that lets users securely view and manage their **GitHub repositories**. Authenticate with GitHub, browse repositories, view insights, and manage issues — all from one interactive dashboard.

-----

## 🧩 Core Technologies

| Category          | Technology                                                 |
| ----------------- | ---------------------------------------------------------- |
| **Framework** | .NET / ASP.NET Core                                        |
| **UI** | Blazor Server                                              |
| **Language** | C\#                                                         |
| **API Integration** | GitHub REST API                                            |
| **API Client** | [Octokit.NET](https://github.com/octokit/octokit.net)      |
| **Authentication**| GitHub OAuth                                               |

-----

## ✨ Key Features

  - 🔐 **Secure GitHub Login** – Integrated with the official **OAuth flow**. Unauthenticated users are automatically redirected to the login page.
  - 📊 **Repository Overview** – Displays a grid-based list of your repositories with key stats like stars, forks, and open issues.
  - 📈 **Repository Details** – Shows recent commits, top contributors, and a list of open issues for a selected repository.
  - 🧠 **Issue Management** – Create new issues or add labels (e.g., `bug`, `enhancement`, `help wanted`) directly from the UI.
  - 🧱 **Clean Architecture** – All GitHub API logic is handled by a dedicated `GitHubService`, keeping Blazor components clean and UI-focused.

-----

## ⚙️ Setup & Configuration

### 1️⃣ Clone the Repository

Clone the project to your local machine and navigate into the directory.

```bash
git clone https://github.com/your-username/github-dashboard.git
cd github-dashboard
```

### 2️⃣ Create a GitHub OAuth App

You need to register a new OAuth application in your GitHub developer settings.

1.  Go to **[GitHub Developer Settings](https://github.com/settings/developers)** and select the **OAuth Apps** tab.
2.  Click **New OAuth App**.
3.  Use the following values for the form:
      * **Application name:** `GitHub Dashboard Local Dev`
      * **Homepage URL:** `https://localhost:7001`
      * **Authorization callback URL:** `https://localhost:7001/signin-github`
4.  Click **Register application**.
5.  Copy the **Client ID** and generate a new **Client Secret**. Save the secret somewhere safe, as GitHub will not show it to you again.

### 3️⃣ Configure Credentials

Add your GitHub OAuth app credentials to the `appsettings.Development.json` file.

```json
{
  "GitHub": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  }
}
```

### 4️⃣ Run the Application

Run the application from your terminal.

```bash
dotnet run
```

Open your browser and navigate to **`https://localhost:7001`** to start using the dashboard.
