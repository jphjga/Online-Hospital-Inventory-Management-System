document.addEventListener("DOMContentLoaded", function () {
    const urlParams = new URLSearchParams(window.location.search);

    // Define default active tabs for left and right columns
    const defaultTabs = {
        left: "products",
        right: "stock-alert"
    };

    // Check if an editId is in the URL to auto-switch to Add Product
    if (urlParams.has("editId")) {
        activateTab("add-product", "left");
    } else {
        activateTab(defaultTabs.left, "left");
        activateTab(defaultTabs.right, "right");
    }

    // Attach click event listeners to all tab links
    document.querySelectorAll(".tab-link").forEach(link => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const column = this.dataset.column; // Determine column (left/right)
            activateTab(this.dataset.tab, column);
        });
    });

    document.addEventListener("DOMContentLoaded", function () {
        document.getElementById("purchases-content").addEventListener("click", function (e) {
            if (e.target.classList.contains("btn-remove")) {
                let productId = e.target.getAttribute("data-id");

                fetch(`?handler=RemovePurchase&productId=${productId}`, { method: "POST" })
                    .then(() => loadPurchases());
            }
        });
    });

    function activateTab(tabId, column) {
        if (!tabId || !column) return;

        // Remove active class from all tab contents within the same column
        document.querySelectorAll(`.tab-content[data-column="${column}"]`).forEach(tab => {
            tab.classList.remove("active");
        });

        // Remove active class from all tab links within the same column
        document.querySelectorAll(`.tab-link[data-column="${column}"]`).forEach(link => {
            link.classList.remove("active");
        });

        // Activate the selected tab content
        const selectedTabContent = document.getElementById(tabId);
        if (selectedTabContent) {
            selectedTabContent.classList.add("active");
        }

        // Activate the selected tab link
        const selectedTabLink = document.querySelector(`.tab-link[data-tab="${tabId}"]`);
        if (selectedTabLink) {
            selectedTabLink.classList.add("active");
        }
    }


    // Auto-switch to Expiry Calendar if clicked from a notification
    document.querySelector(".alert-warning a")?.addEventListener("click", function (e) {
        e.preventDefault();
        activateTab("expiry-calendar", "left");
    });


});
