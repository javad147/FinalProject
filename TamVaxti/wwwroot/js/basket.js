
$(function () {
    $(document).on('click', '.plus', function () {

        let id = parseInt($(this).parent().parent().parent().attr("data-id"));
        let inputValue = $(this).prev();
        let productTotal = $(this).parent().parent().next().next()
        console.log(productTotal)
        $.ajax({
            url: `basket/IncreaseProductCount?id=${id}`,
            type: 'POST',
            success: function (response) {
                $(".rounded-circle").text(response.count);
                $(".basket-total-price").text(`CART ($${response.totalPrice})`)
                $(".total").text(`$${response.totalPrice}`)
                productTotal.html(`${response.productTotalPrice}$`)
                inputValue.val(response.productCount)
            },
        });
    })

    $(document).on('click', '.minus', function () {

        let id = parseInt($(this).parent().parent().parent().attr("data-id"));
        let inputValue = $(this).next();
        let productTotal = $(this).parent().parent().next().next()


        $.ajax({
            url: `basket/DecreaseProductCount?id=${id}`,
            type: 'POST',
            success: function (response) {
                $(".rounded-circle").text(response.count);
                $(".basket-total-price").text(`CART ($${response.totalPrice})`)
                $(".total").text(`$${response.totalPrice}`)
                productTotal.html(`${response.productTotalPrice}$`)
                inputValue.val(response.productCount)
            },
        });
    })


    $(document).on('click', '.delete-btn', function () {

        let id = parseInt($(this).parent().parent().attr("data-id"));
        console.log(id)

        $.ajax({
            url: `basket/DeleteProduct?id=${id}`,
            type: 'POST',
            success: function (response) {
                $(".rounded-circle").text(response.count);
                $(".basket-total-price").text(`CART ($${response.totalPrice})`);
                $(".total").text(`$${response.totalPrice}`);
                if (response.count == 0) {
                    $(".cart-table").removeClass("d-block");
                    $("#cart-products .container").html(`<div class=" text-center alert alert-warning cart-alert mt-4" role="alert">Products not added yet</div>`)
                }
                $(`[data-id = ${id}]`).remove();
            },
        });
    })
})