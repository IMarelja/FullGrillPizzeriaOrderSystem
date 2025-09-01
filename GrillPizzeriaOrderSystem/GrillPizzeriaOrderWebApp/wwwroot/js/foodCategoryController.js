// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.LoadFoodCategories = function (callback) {
    $.ajax({
        url: '/FoodCategory/GetAll',
        dataType: "json",
        type: "GET",
        cache: false,
        success: function (response) {
            if (response.success) {
                var categories = response.data;
                console.log('Categories loaded:', categories);
                if (callback) callback(categories);
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, err) {
            console.error('AJAX error:', status, err, 'HTTP:', xhr.status, 'Response:', xhr.responseText);
            alert('AJAX: Error loading food categories');
        }
    });
};