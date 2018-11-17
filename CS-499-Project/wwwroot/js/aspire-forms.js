var body = document.getElementsByTagName("body")[0];
body.style.backgroundImage = "url('/images/form-leaf-background.jpg')";
body.style.backgroundRepeat = "no-repeat";
body.style.backgroundSize = "cover";
body.style.display = "flex";
body.style.alignItems = "center";
body.style.justifyContent = "center";
body.style.backgroundAttachment = "fixed";


$(document).ready(function () {
    $("#interest").hide();

    $('#accountType').on('change', function () {
        if (($('[name="type"]').val() != '0') && ($('[name="type"]').val() != undefined)) { // zero is checking
            console.log($('#acctType').val());
            $("#interest").show();
        }
        else {
            $("#interest").hide();
        }
    });

});