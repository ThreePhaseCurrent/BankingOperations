$(document).ready(function() {
    $("#transferFormId").submit(function(e) {

        const senderId = $("#formGroupExampleInput").innerText;
        console.log(senderId);

        $('.toast').toast('show');
    });

    $('.toast').toast('show');
});