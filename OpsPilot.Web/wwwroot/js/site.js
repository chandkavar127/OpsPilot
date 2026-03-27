const statusElement = document.getElementById("notificationStatus");

if (window.signalR && statusElement) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/notifications")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveNotification", (title, message) => {
        alert(`${title}: ${message}`);
    });

    connection.start()
        .then(() => statusElement.textContent = "Connected")
        .catch(() => statusElement.textContent = "Disconnected");
}
