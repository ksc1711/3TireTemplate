//로그인
function memberLogin() {
    if (loginVaildation()) {

        var id = $('#txt_userId').val();
        var pw = $('#txt_userPw').val();

        $.ajax({
            url: "/Account/Login",
            type: "POST",
            data: {
                memberId: id,
                memberPw: pw,
            },
            success: function () {
                window.location.replace("/Home/Index");
            },
            error: function (a, b, c) {
                alert("Please check your id and password.");
                console.log(all);
                console.log(type);
                console.log(message);
            }
        });
    }
}

function moveSignUp() {
    window.location.replace("/Account/Register");
}

//로그인 유효성 검사 
function loginVaildation() {
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

// 회원가입
function memberSignUp() {
    if (signUpVaildation()) {

        var memberId = $('#txt_userId').val();
        var memberPw = $('#txt_userPw').val();
        var memberName = $('#txt_userName').val();
        var memberPhone = $('#txt_userPhone').val();

        $.ajax({
            url: "/Account/Register",
            type: "POST",
            data: {
                memberId: memberId
                ,memberPw: memberPw
                ,memberName: memberName
                ,memberPhone: memberPhone
            },
            success: function () {
                alert("suess");
                //window.location.replace("/Home/Index");
            },
            error: function (all, type, message) {
                alert("Please check your id and password.");
                console.log(all);
                console.log(type);
                console.log(message);
            }
        });
    }
}

// 회원가입 유효성검사 
function signUpVaildation() {
    $('#p_userId').text("");
    $('#p_userPw').text("");
    $('#p_chk_userPw').text("");
    $('#p_userName').text("");
    $('#p_userPhone').text("");

    var pattern = /[-]/; 

    if ($('#txt_userId').val() == "") {
        $('#p_userId').text("Please enter your id.");
        return;
    }
    else if ($('#txt_userId').val().length > 20) {
        $('#p_userId').text("id is 20 characters maximum.");
        return;
    }

    if ($('#txt_userPw').val() == "") {
        $('#p_userPw').text("Please enter your password.");
        return;
    } else if ($('#txt_userPw').val().length > 20) {
        $('#p_userId').text("Passowrd is 20 characters maximum.");
        return;
    }

    if ($('#txt_chk_userPw').val() == "") {
        $('#p_chk_userPw').text("Please enter your checkPassword.");
        return;
    }
    else if ($('#txt_userPw').val() != $('#txt_chk_userPw').val()) {
        $('#p_chk_userPw').text("Not same Password.");
        return;
    }

    if ($('#txt_userName').val() == "") {
        $('#p_userName').text("Please enter your Name.");
        return;
    }

    if ($('#txt_userPhone').val() == "") {
        $('#p_userPhone').text("Please enter your PhoneNumber.");
        return;
    }
    else if (!pattern.test($('#txt_userPhone').val())) {
        $('#p_userPhone').text("Please check the input format.(XXX-XXXX-XXXX)");
        return;
    }

    return true;
}
