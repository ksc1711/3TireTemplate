//�α���
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
            success: function (data) {
                switch (data) {
                    case "S": window.location.replace("/Home/Index"); break;
                    case "F": 
                    case "V": 
                    default: alert("Please check your id and password."); break;
                }
            },
            error: function (all, type, message) {
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

//�α��� ��ȿ�� �˻� 
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

// ȸ������
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
            success: function (data) {
                switch (data) {
                    case "S": window.location.replace("/Account/Login"); break;
                    case "F": alert("Duplicate ID."); break;
                    case "V": alert("Invalid input."); break;
                    default: alert("Please check your id and password."); break;
                }
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

// ȸ������ ��ȿ���˻� 
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
    else if ($('#txt_userPhone').val().length != 13) {
        $('#p_userPhone').text("Phone Number Length is 13");
        return;
    }

    return true;
}
