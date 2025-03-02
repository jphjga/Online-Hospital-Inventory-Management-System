document.addEventListener("DOMContentLoaded", function () {
    const navLinks = document.querySelectorAll(".report-nav a");
    const sections = document.querySelectorAll(".report-section");

    navLinks.forEach(link => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            // Remove active classes
            navLinks.forEach(l => l.classList.remove("active"));
            sections.forEach(section => section.classList.remove("active"));

            // Activate clicked link and its section
            this.classList.add("active");
            const targetId = this.getAttribute("data-target");
            const targetSection = document.getElementById(targetId);
            if (targetSection) {
                targetSection.classList.add("active");
            }
        });
    });

    // Activate the first section by default
    if (navLinks.length > 0) {
        navLinks[0].click();
    }
});
document.addEventListener("DOMContentLoaded", function () {
    const ctx = document.getElementById('salesTrendChart');
    if (ctx) {
        // Use the global variable that was set in the Razor view
        const labels = salesTrendData.map(item => item.Month + "/" + item.Year);
        const dataValues = salesTrendData.map(item => item.Total);

        const salesTrendChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Sales Trend',
                    data: dataValues,
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
});
document.addEventListener("DOMContentLoaded", function () {   
    const invCtx = document.getElementById('inventoryChart');
    if (invCtx && typeof inventoryData !== "undefined") {
        const labels = inventoryData.map(item => item.Category);
        const totalStocks = inventoryData.map(item => item.TotalStock);

        const inventoryChart = new Chart(invCtx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Total Stock by Category',
                    data: totalStocks,
                    backgroundColor: 'rgba(75, 192, 192, 0.5)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

   
});
//purchase summary chart
document.addEventListener("DOMContentLoaded", function () {
    // Render Purchase Trend Chart if the canvas exists
    const purchaseCtx = document.getElementById('purchaseTrendChart');
    if (purchaseCtx && typeof purchaseTrendData !== "undefined") {
        const labels = purchaseTrendData.map(item => item.Month + "/" + item.Year);
        const dataValues = purchaseTrendData.map(item => item.Total);
        const purchaseTrendChart = new Chart(purchaseCtx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Purchase Trend',
                    data: dataValues,
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
    }
});


// Toggle the navigation bar open/closed
function toggleNav() {
    const nav = document.querySelector('.report-nav');
    const content = document.querySelector('.report-content');
    if (nav.classList.contains('collapsed')) {
        // Expand the navigation bar
        nav.classList.remove('collapsed');
        content.classList.remove('expanded');
    } else {
        // Collapse the navigation bar
        nav.classList.add('collapsed');
        content.classList.add('expanded');
    }
}
