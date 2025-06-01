# SharpPortScanner

SharpPortScanner is a lightweight, single-file C# Windows Forms application that performs multi-threaded TCP port scanning on a target host within a specified port range. It offers an intuitive GUI, simple operation, and real-time status updates.

---

## 📌 Project Summary

- **Type**: Single-file WinForms App (.NET Framework)
- **Language**: C#
- **Function**: TCP Port Scanner (synchronous GUI + async network calls)
- **Design**: Simple UI with minimal controls, responsive status feedback

---

## ✨ Features

- Enter target IP or hostname
- Specify start and end port
- Real-time scanning status updates
- Open/Closed port identification
- Multi-threaded port scanning
- UI remains responsive during scan
- No external libraries required
- Easy to compile and run
- Pure C#, no unsafe code
- Includes error handling
- Designed for local network and educational scanning

---

## 🧰 Requirements

- Windows OS (7/8/10/11)
- .NET Framework 4.7 or later
- Visual Studio (Community/Pro/Enterprise) or compatible IDE

---

## 🧱 Architecture

The program is structured using a single `Form` class that:

- Builds the entire UI inside the constructor
- Uses native `TcpClient` from `System.Net.Sockets` for connection testing
- Uses `Task.Run` and `SemaphoreSlim` to perform parallel scans
- Updates UI from async threads using `Invoke(...)`
- Outputs results in a `ListBox`
- Displays real-time progress using a `Label`
- Uses `try-catch` for safe networking

---

## 🔄 How It Works

1. User inputs target and port range.
2. When `Start Scan` is clicked:
   - Ports are validated.
   - `ScanPorts(...)` starts a loop.
   - Each port is scanned using a separate `Task`.
   - Semaphore limits concurrency.
   - `TcpClient.ConnectAsync(...)` checks if port is open.
   - Each result is posted to the UI list box.
   - Status label shows progress as "Scanned X/Y ports..."

---

## 🧪 Example Usage

- **Target IP**: 127.0.0.1
- **Start Port**: 20
- **End Port**: 80

Example result in list box:

[OPEN] Port 21
[CLOSED] Port 22
[OPEN] Port 23
[CLOSED] Port 24

---

## 🔍 UI Elements

| Element         | Description                          |
|-----------------|--------------------------------------|
| `TextBox` Host  | Input for IP or hostname             |
| `TextBox` Start | Start port (integer)                 |
| `TextBox` End   | End port (integer)                   |
| `Button` Scan   | Starts scanning process              |
| `ListBox` Log   | Displays scanned port results        |
| `Label` Status  | Displays scanning progress           |

---

## 📤 Output Format

Each scanned port results in a formatted string in the list box:
[OPEN] Port 443
[CLOSED] Port 445

Results appear incrementally as the scan progresses.

---

## 🛑 Error Handling

- Invalid port numbers: prompts user to re-enter range
- Invalid range (start > end): handled before scanning
- Empty host/IP: does nothing
- Invalid IP format: TCP connect simply fails safely
- Connect timeouts handled via `Task.WhenAny(...)`

---

## ⚙️ Performance

- Supports scanning full range `1–65535`
- Uses up to 100 concurrent tasks (throttle with `SemaphoreSlim`)
- UI thread remains responsive
- Minimal CPU usage due to async operations
- Completion time depends on range and network conditions

---

## 🧠 Core Code Concepts

### Async TCP Scan

```csharp
bool open = await IsPortOpen(host, port, timeout);
Concurrency Management

SemaphoreSlim throttle = new SemaphoreSlim(100);

Thread-safe UI Updates

this.Invoke(new Action(() => {
    lstResults.Items.Add(...);
}));

🔄 Function Breakdown
Function	Purpose
Main()	Launch WinForms application
PortScannerForm()	Construct and layout GUI
BtnScan_Click()	Handles click event for scan button
ScanPorts(...)	Main scanning loop with tasks
IsPortOpen(...)	TCP connection test to given port
📂 Deployment

To distribute:

    Build with Release configuration

    Navigate to /bin/Release

    Distribute .exe file

    Can be run standalone (no installer required)

💡 Common Use Cases

    Identify which services are running on a LAN device

    Test open ports on a local server

    Network diagnostics in isolated environments

    Security research and validation

    IT students learning about sockets

🔐 Security Notice

This tool is intended for educational and testing purposes only.

    Do not use this tool on unauthorized systems

    Always have permission from the target host owner

    Scanning external systems may violate ISP or legal policies
