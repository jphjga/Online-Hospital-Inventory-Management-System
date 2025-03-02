document.addEventListener("DOMContentLoaded", function () {
    const userProfileModal = document.getElementById("userProfileModal");
    const manageUserModal = document.getElementById("manageUserModal");

    window.openUserProfileModal = function () {
        userProfileModal.classList.add("show");
    };

    window.openManageUserModal = function () {
        userProfileModal.classList.remove("show"); // Close user profile modal
        manageUserModal.classList.add("show"); // Open manage user modal
    };

    window.closeManageUserModal = function () {
        manageUserModal.classList.remove("show");
    };

    window.logoutUser = function () {
        window.location.href = "/Index"; // Redirect to login page
    };

    // Close modal when clicking outside
    window.onclick = function (event) {
        if (event.target === userProfileModal) {
            userProfileModal.classList.remove("show");
        }
        if (event.target === manageUserModal) {
            manageUserModal.classList.remove("show");
        }
    };

    // Check if the "updatePasswordBtn" element exists before attaching the event listener
    const updatePasswordBtn = document.getElementById("updatePasswordBtn");
    if (updatePasswordBtn) {
        updatePasswordBtn.addEventListener("click", function () {
            // Get the new password and other details from sessionStorage or inputs
            let newPassword = document.getElementById("newPassword").value;
            let userId = sessionStorage.getItem("UserId");
            let institutionId = sessionStorage.getItem("InstitutionId");
            let username = sessionStorage.getItem("Username");
            let email = sessionStorage.getItem("Email");
            let errorElement = document.getElementById("passwordError");

            // Validate password length
            if (!newPassword || newPassword.length < 4) {
                errorElement.style.display = "block";
                return;
            }

            errorElement.style.display = "none"; // Hide error if valid

            // Prepare the form data (URL-encoded)
            const formData = new URLSearchParams();
            formData.append("userId", userId);
            formData.append("institutionId", institutionId);
            formData.append("username", username);
            formData.append("email", email);
            formData.append("newPassword", newPassword);

            // Send the request using URL-encoded form data
            fetch("/MainDash?handler=UpdatePassword", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                },
                body: formData.toString()  // Serialize the form data
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        alert("Password updated successfully!");
                        closeManageUserModal(); // Close the modal on success
                    } else {
                        alert("Error: " + data.errorMessage);  // Display error message
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    alert("An error occurred while updating the password.");
                });
        });
    } else {
        console.warn("Password update button (updatePasswordBtn) not found.");
    }


    window.openQueryCenterModal = function () {
        const modal = document.getElementById("queryCenterModal");
        if (modal) {
            modal.classList.add("show");
            console.log("Query Center modal opened.");
        } else {
            console.warn("Query Center modal element not found.");
        }
    };

    window.closeQueryCenterModal = function () {
        const modal = document.getElementById("queryCenterModal");
        if (modal) {
            modal.classList.remove("show");
            // Reset modal content
            const searchInput = document.getElementById("searchQuery");
            const resultsContainer = document.getElementById("searchResults");
            if (searchInput) searchInput.value = "";
            if (resultsContainer) resultsContainer.innerHTML = "";
            console.log("Query Center modal closed and content reset.");
        }
    };

    // --- Search Function ---
    window.searchProducts = function () {
        const query = document.getElementById("searchQuery").value.trim();
        const resultsContainer = document.getElementById("searchResults");

        if (!query) {
            resultsContainer.innerHTML = "<p>Please enter a search term.</p>";
            return;
        }

        resultsContainer.innerHTML = "<p>Searching...</p>";

        fetch("/MainDash?handler=SearchProducts&q=" + encodeURIComponent(query))
            .then(response => response.json())
            .then(data => {
                let html = "";
                if (data && data.length > 0) {
                    data.forEach(product => {
                        html += `<div class="search-result">
                               <h3>${product.name}</h3>
                               <p>${product.description}</p>
                               <p><em>Institution: ${product.institutionName}</em></p>
                               <button class="expand-btn" onclick="toggleExpand(this)">Expand</button>
                               <div class="additional-info" >
                                   <p><strong>Location info:</strong><br> ${product.institutionLocation || 'N/A'}</p>
                                   <p><strong>About:</strong><br> ${product.institutionAbout || 'N/A'}</p>
                               </div>
                             </div>`;
                    });
                } else {
                    html = "<p>No products found.</p>";
                }
                resultsContainer.innerHTML = html;
            })
            .catch(error => {
                console.error("Error searching products:", error);
                resultsContainer.innerHTML = "<p>An error occurred while searching.</p>";
            });
    };

   window.toggleExpand = function(btn) {
    // Get the additional info container that follows the button
    const infoDiv = btn.nextElementSibling;
       if (infoDiv.classList.contains("expanded")) {
              // Collapse the container
        infoDiv.classList.remove("expanded");
        btn.textContent = "Expand";
    } else {
        // Expand the container      
        infoDiv.classList.add("expanded");
        btn.textContent = "Collapse";
    }
};


});



