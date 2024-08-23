function initializeWishlist() {
    // Load wishlist from cookies
    var wishlist = []; // Cookies.getJSON('wishlist') || [];
    $.getJSON("/WishList/GetUserSavedWishList",
        (data) => {
            wishlist = data;
            $(".add-to-wishlist").each(function() {
                var productId = $(this).data("sku-id");

                var $icon = $(this).find("i");
                if (wishlist.includes(productId)) {
                    $icon.removeClass("ti-heart").addClass("fa fa-fw fa-heart");
                }
            });
        });
    // Apply wishlisted class to already wishlisted items


    // Handle add to wishlist click
    $(document).on("click",
        ".add-to-wishlist",
        function() {
            var productId = $(this).data("sku-id");
            var $icon = $(this).find("i");
            var index = wishlist.indexOf(productId);
            if (index !== -1) {
                // Remove from wishlist

                $.getJSON("/WishList/RemoveProductFromWishList?productId=" + productId,
                    (data) => {
                        wishlist = data;
                        wishlist.splice(index, 1);
                        $icon.removeClass("fa fa-fw fa-heart").addClass("ti-heart");
                        notifyWishlist(false);
                        refreshWishListView();
                    });


            } else {
                $.getJSON("/WishList/AddToWishList?productId=" + productId,
                    (data) => {
                        wishlist = data;
                        wishlist.push(productId);
                        $icon.removeClass("ti-heart").addClass("fa fa-fw fa-heart");
                        notifyWishlist(true);
                        refreshWishListView();
                    });
            }
            // Update the cookie
            Cookies.set("wishlist", wishlist, { expires: 7 });
        });
}

function initializeCart() {
    // Load cart from cookies
    var cart = Cookies.getJSON("cart") || {};

    // Handle add to cart click
    $(document).on("click",
        ".add-to-cart",
        function() {
            cart = Cookies.getJSON("cart") || {};
            var productId = $(this).data("sku-id");

            if (cart[productId]) {
                // Increment the count of the product in the cart
                cart[productId]++;
            } else {
                // Add new product to the cart with count 1
                cart[productId] = 1;
            }
            console.log(cart);
            notifyCart(true);
            // Update the cookie
            Cookies.set("cart", cart, { expires: 7 });
            refreshCartView();
        });

    $(document).on("change",
        ".cart-product-counter",
        function() {
            cart = Cookies.getJSON("cart") || {};
            var productId = $(this).data("sku-id");
            var index = Object.keys(cart).findIndex(key => key === productId.toString());
            if (cart[productId]) {
                cart[productId] = $(this).val();
            }
            // Update the cookie
            Cookies.set("cart", cart, { expires: 7 });
            refreshCartView();
        });

    $(document).on("click",
        ".remove-from-cart",
        function() {
            cart = Cookies.getJSON("cart") || {};
            var productId = $(this).data("sku-id");
            var index = Object.keys(cart).findIndex(key => key === productId.toString());
            if (cart[productId]) {
                delete cart[productId];
            }
            notifyCart(false);
            // Update the cookie
            Cookies.set("cart", cart, { expires: 7 });
            $(this).closest("li").remove();
            refreshCartView();
        });
}

function notifyWishlist(added) {
    var message = added ? "Item Successfully added in wishlist" : "Item Successfully removed from wishlist";
    $.notify({
            icon: "fa fa-check",
            title: "Success!",
            message: message
        },
        {
            element: "body",
            position: null,
            type: "info",
            allow_dismiss: true,
            newest_on_top: false,
            showProgressbar: true,
            placement: {
                from: "top",
                align: "right"
            },
            offset: 20,
            spacing: 10,
            z_index: 1031,
            delay: 5000,
            animate: {
                enter: "animated fadeInDown",
                exit: "animated fadeOutUp"
            },
            icon_type: "class",
            template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
                '<button type="button" aria-hidden="true" class="btn-close" data-notify="dismiss"></button>' +
                '<span data-notify="icon"></span> ' +
                '<span data-notify="title">{1}</span> ' +
                '<span data-notify="message">{2}</span>' +
                '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                "</div>" +
                '<a href="{3}" target="{4}" data-notify="url"></a>' +
                "</div>"
        });
}

function notifyCart(added) {
    var message = added ? "Item Successfully added to cart" : "Item removed from the cart";
    $.notify({
            icon: "fa fa-check",
            title: "Success!",
            message: message
        },
        {
            element: "body",
            position: null,
            type: "info",
            allow_dismiss: true,
            newest_on_top: false,
            showProgressbar: true,
            placement: {
                from: "top",
                align: "right"
            },
            offset: 20,
            spacing: 10,
            z_index: 1031,
            delay: 5000,
            animate: {
                enter: "animated fadeInDown",
                exit: "animated fadeOutUp"
            },
            icon_type: "class",
            template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
                '<button type="button" aria-hidden="true" class="btn-close" data-notify="dismiss"></button>' +
                '<span data-notify="icon"></span> ' +
                '<span data-notify="title">{1}</span> ' +
                '<span data-notify="message">{2}</span>' +
                '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
                "</div>" +
                '<a href="{3}" target="{4}" data-notify="url"></a>' +
                "</div>"
        });
}

function refreshCartView() {
    $.get("/Basket/CartPartial",
        function(data) {
            $("#cart-container").html(data);

        });

    $.get("/Basket/SideCartPartial",
        function(data) {
            $("#cart_side").html(data);
        });
}

function refreshWishListView() {
    $.get("/Product/WishListProducts",
        (data) => {
            $("#wishlist-products").html(data);
        });
}

function initializeCurrency() {
    // Load currency from cookies
    var currency = Cookies.getJSON("currency") || {};

    // Handle currency click
    $(document).on("click",
        ".show-currency",
        function () {
            currency = Cookies.getJSON("currency") || {};
            if (currency) {
                delete currency;
            }
            currency = $(this).data("currency-id");
            $.get("/Product/SetCurrencyRate?symbol=" + currency,
                (data) => {

                    //$("#wishlist-products").html(data);
                });
            console.log(currency);
            // Update the cookie
            Cookies.set("currency", currency, { expires: 7 });

            window.location.reload(true);
        });

}

$(function () {
    initializeCurrency();
    initializeWishlist();
    initializeCart();
});