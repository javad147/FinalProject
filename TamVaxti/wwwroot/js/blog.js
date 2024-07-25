$(function () {
    $(document).on("click", ".show-more button", function () {

        let skip = $(".blogs-area").children().length;

        let count = $(".blogs-area").attr('data-count')

        let blogs = $(".blogs-area");

        let blogsContent = $(".blogs-area").html();

        $.ajax({
            url: `Blog/showmore?skip=${skip}`,
            type: 'GET',
            success: function (response) {
                blogsContent += response;
                $(blogs).html(blogsContent);

                skip = $(".blogs-area").children().length;
                if (skip >= count) {
                    $(".show-more button").addClass("d-none");
                }
            },
        });
    })
});