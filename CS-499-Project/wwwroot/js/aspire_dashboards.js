$(document).ready(function () {
    $("#user-trash").click(function ()
    {
        var input = prompt("Are you sure you want to delete this user?\nType \"DELETE\" (case sensitive without quotes) to remove this user.");
        if (input === "DELETE")
        {
            window.location.assign($(this).data("url"));
        }
    });
});

