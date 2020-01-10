function memberLogin() {
    if (memberVaildation()) {

        var id = $('#txt_userId').val();
        var pw = $('#txt_userPw').val();

        $.ajax({
            url: "/Account/Login",
            type: "POST",
            data: {
                memberId: id,
                memberPw: pw,
            },
            error: function (a, b, c) {
                alert("Please check your id and password.");
                console.log(a);
                console.log(b);
                console.log(c);
            }
        })
    }
    
    //alert("Please check your id and password.");
    
}

function memberVaildation() {
    $('#p_userId').text("");
    $('#p_userPw').text("");
    
    if ($('#txt_userId').val() == "") {
        $('#p_userId').text("Please enter your id.");
        return;
    }

    if ($('#txt_userPw').val() == "") {
        $('#p_userPw').text("Please enter your password.");
        return;
    }

    return true;

}