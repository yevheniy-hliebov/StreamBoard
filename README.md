# <img src="https://raw.githubusercontent.com/yevheniy-hliebov/StreamTabula/refs/heads/develop/Desktop/Assets/Images/logo.png" width="100" height="100" alt="StreamTabula Logo" align="center"/> StreamTabula

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/Framework-WPF%20%7C%20.NET-512BD4)
![Status](https://img.shields.io/badge/Status-Beta-orange)

**StreamTabula** is a modern, highly customizable desktop application designed to build and manage macro pads and virtual stream decks. Whether you're a streamer, video editor, or power user, StreamTabula allows you to automate tasks and bind complex actions to simple button presses.

## ✨ Key Features

* **Visual Deck Editor:** An intuitive drag-and-drop interface for configuring button grids and pages.
* **Action Engine:** Assign a wide variety of actions (hotkeys, application launches, system commands) to any button.
* **Advanced Clipboard:** Professional workflow support with Cut, Copy, Paste, and Duplicate for buttons, pages, and actions.
* **Modern Windows UI:** Built with WPF UI, featuring Windows 11 Mica materials, smooth animations, and Light/Dark themes.
* **System Integrations:** 
  * Minimize to System Tray
  * Run on Windows Startup
  * Run as Administrator support
* **Local Data Storage:** All settings, decks, and profiles are safely stored as JSON files.

## 🚀 Getting Started

### Prerequisites
* Windows 10 or Windows 11.
* .NET Runtime (Version specified in the project).

### Installation
## Windows (Desktop application)
1. Go to the [Releases](../../releases) page.
2. Download the latest `.zip` file.
3. Extract and run `StreamTabula.exe`.

## Android (Client application)
1. Go to the [Releases](../../releases) page.
2. Download the latest `.apk` file and run to install.

## 🛠️ Architecture & Tech Stack

StreamTabula is built with a strong emphasis on clean architecture and performance:
* **C# / .NET**
* **WPF (Windows Presentation Foundation)**
* **MVVM Pattern:** Using `ObservableObject` and `Microsoft.Extensions.DependencyInjection` for clean separation of concerns.
* **Key Libraries:**
  * [WPF UI](https://github.com/lepoco/wpfui) - Modern UI controls and styles.
  * [GongSolutions.Wpf.DragDrop](https://github.com/punker76/gong-wpf-dragdrop) - Seamless drag-and-drop mechanics.
  * [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon) - Robust system tray integration.

## 🤝 Contributing

This project is currently in Beta. Feedback, bug reports, and pull requests are highly appreciated! 
1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'feat(module): add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

Distributed under the MIT License. See [`LICENSE`](https://github.com/yevheniy-hliebov/StreamTabula?tab=MIT-1-ov-file) for more information.