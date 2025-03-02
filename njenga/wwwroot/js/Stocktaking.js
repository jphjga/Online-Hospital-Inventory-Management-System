document.addEventListener("DOMContentLoaded", function () {  
    function updateTimeDate() {
        const now = new Date();
        const options = {
            year: 'numeric', month: 'short', day: 'numeric',
            hour: 'numeric', minute: 'numeric', second: 'numeric'
        };
        document.getElementById("currentTimeDate").textContent = now.toLocaleString('en-US', options);
    }
    updateTimeDate();
    setInterval(updateTimeDate, 1000);
});
