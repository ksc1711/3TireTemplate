// 날짜 포맷 리턴
/*
예) new Date(1).format("yyyy-MM-dd");
*/
Date.prototype.format = function (f) {
    if (!this.valueOf() || this.valueOf() < 0) return " ";

    var weekName = ["일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일"];
    var d = this;

    return f.replace(/(yyyy|yy|MM|dd|E|hh|mm|ss|a\/p)/gi, function ($1) {
        switch ($1) {
            case "yyyy": return d.getFullYear();
            case "yy": return (d.getFullYear() % 1000).zf(2);
            case "MM": return (d.getMonth() + 1).zf(2);
            case "dd": return d.getDate().zf(2);
            case "E": return weekName[d.getDay()];
            case "HH": return d.getHours().zf(2);
            case "hh": return ((h = d.getHours() % 12) ? h : 12).zf(2);
            case "mm": return d.getMinutes().zf(2);
            case "ss": return d.getSeconds().zf(2);
            case "a/p": return d.getHours() < 12 ? "오전" : "오후";
            default: return $1;
        }
    });
};

Array.prototype.checkIfArrayIsUnique = function () {
    this.sort();

    for (var i = 1; i < this.length; i++) {
        if (this[i - 1] == this[i])
            return false;
    }
    return true;
}

Array.prototype.checkIfArrayIsUniqueExpectZero = function () {
    this.sort();

    for (var i = 1; i < this.length; i++) {
        if (this[i] == 0)
            continue;
        if (this[i - 1] == this[i])
            return false;
    }
    return true;
}


//JSON Date Type Convert
String.prototype.ToShortScriptDate = function () {
    return new Date(parseInt(this.replace("/Date(", "").replace(")/", ""), 10)).format("dd.MM.yyyy");
}

//JSON Date Type Convert
String.prototype.ToLongScriptDate = function () {
    return new Date(parseInt(this.replace("/Date(", "").replace(")/", ""), 10)).format("dd.MM.yyyy HH:mm");
}

//JSON Date Type Convert
String.prototype.ToTinyScriptDate = function () {
    return new Date(parseInt(this.replace("/Date(", "").replace(")/", ""), 10)).format("dd.MM");
}

//JSON CultureCode -> CultureName
String.prototype.ToCultureName = function () {
    if (this == null)
        return "";
    
    switch (this.Trim().toUpperCase()) {
        case "ID-ID":
            return "Indonesian";
        case "EN-US":
            return "English";
        default:
            return "";
    }
}

//JSON CultureName -> CultureCode
String.prototype.ToCultureCode = function () {
    if (this == null)
        return "";

    switch (this.Trim()) {
        case "Indonesian":
            return "id-ID";
        case "English":
            return "en-US";
        default:
            return "";
    }
}

// String Trim
String.prototype.Trim = function () {
    return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
}

String.prototype.string = function (len) { var s = '', i = 0; while (i++ < len) { s += this; } return s; };
String.prototype.zf = function (len) { return "0".string(len - this.length) + this; };
Number.prototype.zf = function (len) { return this.toString().zf(len); };

Date.prototype.ToAddDayShortDate = function (pAddDay) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + pAddDay);
    return date.format("dd.MM.yyyy");
}


//Double Click Block
document.ondblclick = function (evt) {
    if (window.getSelection)
        window.getSelection().removeAllRanges();
    else if (document.selection)
        document.selection.empty();
}

function isNumber(s) {
    s += ''; // 문자열로 변환
    s = s.replace(/^\s*|\s*$/g, ''); // 좌우 공백 제거
    if (s == '' || isNaN(s)) return false;
    return true;
}

//only number
function onlyNumber() {
    if (typeof (event.which) == "undefined") {
        //ie10
        var ek = event.keyCode;
        if ($.inArray(ek, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (ek == 65 && (event.ctrlKey === true || event.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (ek >= 35 && ek <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((event.shiftKey || (ek < 48 || ek > 57)) && (ek < 96 || ek > 105))
            event.returnValue = false;
    }
    else {
        //ie11
        var ek = event.which;
        if ($.inArray(ek, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (ek == 65 && (event.ctrlKey === true || event.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (ek >= 35 && ek <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((event.shiftKey || (ek < 48 || ek > 57)) && (ek < 96 || ek > 105))
            event.preventDefault();
    }
}

function onlyNumberAndBackSpace() {
    //alert("key : " + event.keyCode);
    if ((event.keyCode < 96 || event.keyCode > 105) && (event.keyCode < 48 || event.keyCode > 57) && event.keyCode != 8) {
        event.returnValue = false;
        if (event.preventDefault) event.preventDefault();
    }
}

function onlyNumberAndBackSpaceAndDot() {
    //alert("key : " + event.keyCode);
    if ((event.keyCode < 96 || event.keyCode > 105) && (event.keyCode < 48 || event.keyCode > 57) && event.keyCode != 8 && event.keyCode != 190) {
        event.returnValue = false;
        if (event.preventDefault) event.preventDefault();
    }
}

function onlyAlphabetsByKeyUp(obj) {
    val = obj.value;
    re = /[^a-zA-Z]/gi;
    obj.value = val.replace(re, "").toUpperCase();
}

function onlyAlphabets() {
    //alert("key : " + event.keyCode);
    if ((event.keyCode >= 65 && event.keyCode <= 90) || (event.keyCode >= 97 && event.keyCode <= 122)) {
        event.returnValue = true;
    }
    else {
        event.returnValue = false;
        if (event.preventDefault) event.preventDefault();
    }
}

var GetHTML = function (id, contoller, action, data) {
    var returnFlag = true;

    if (data) {
        $.ajax({
            url: '/' + contoller + '/' + action,
            type: "POST",
            dataType: "html",
            data: data,
            async: false,
            success: function (data) {
                if (data) {
                    $("#" + id).html(data);
                }
            },
            error: function (request, status, error) {
                AlertMsg("Window Alert", "Please contact your administrator.");
                returnFlag = false;
            }
        });
    }
    else {
        $.ajax({
            url: '/' + contoller + '/' + action,
            type: "POST",
            dataType: "html",
            async: false,
            success: function (data) {
                if (data) {
                    $("#" + id).html(data);
                }
            },
            error: function (request, status, error) {
                AlertMsg("Window Alert", "Please contact your administrator.");
                returnFlag = false;
            }
        });
    }

    return returnFlag;
};

var RemoveCommaStr = function (value) {
    var returnValue = value.replace(/,/g, "");
    return returnValue;
}

var RemoveCommaNumber = function (value) {
    var returnValue = value.replace(/,/g , "");
    return Number(returnValue) == isNaN ? 0 : Number(returnValue);
}

function NumberWithCommas(value) {
    var parts = value.toString().split(".");
    var firstValue = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    var secondValue = (parts[1] ? "." + parts[1] : "");
    return firstValue + secondValue;
}

/// <summary>
/// 
/// </summary>
var ConfirmMsg = function (pEvent, pTitle, pConfirmMsg, pCallback, pCallbackParam1, pCallbackParam2) {
    $.SmartMessageBox({
        title: pTitle,
        content: pConfirmMsg,
        buttons: '[No][Yes]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Yes") {
            if (pCallback != undefined) {
                if (pCallbackParam1 != undefined && pCallbackParam2 == undefined)
                    pCallback(pCallbackParam1);
                else if (pCallbackParam1 != undefined && pCallbackParam2 != undefined)
                    pCallback(pCallbackParam1, pCallbackParam2);
                else {
                    pCallback();
                }
            }
        }
    });
    
    if (pEvent != null && pEvent != undefined)
        pEvent.preventDefault();
}

var AlertMsg = function (pTitle, pAlertMsg, pEvent, pCallback, pCallbackParam) {
    $.SmartMessageBox({
        title: pTitle,
        content: pAlertMsg,
        buttons: '[Yes]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Yes") {
            if (pCallback) {
                if (pCallbackParam != undefined)
                    pCallback(pCallbackParam);
                else {
                    pCallback();
                }
            }
        }
    });

    if (pEvent != null && pEvent != undefined)
        pEvent.preventDefault();
}


var ResetSelectBox = function (pId) {
    $("#" + pId + " option").not("[value='']").remove();
}

var EnterKeyPress = function (pEvent, pCallback, pCallbackParam) {
    if(pEvent.keyCode == 13)
    {
        if (pCallback)
        {
            if (pCallbackParam)
                pCallback(pCallbackParam);
            else
                pCallback();
        }
    }
}

function NumCalc(pNum1, pNum2, pCal) {
    var patten = /\./;
    if (!isNaN(pNum1) && !isNaN(pNum2)) {
        if (patten.test(pNum1)) {
            var pNum1 = pNum1.toString();
            var a1 = pNum1.split(".");
            var b1 = a1[1].length;
        } else {
            var b1 = 0;
            pNum1 = parseInt(pNum1, 10);
        }
        if (patten.test(pNum2)) {
            var pNum2 = pNum2.toString();
            var a2 = pNum2.split(".");
            var b2 = a2[1].length;
        } else {
            var b2 = 0;
            pNum2 = parseInt(pNum2, 10);
        }
        var c1 = 1, c2 = 1;
        for (var i = 0 ; i < b1 ; i++) {
            c1 = c1 * 10;
        }
        for (var i = 0 ; i < b2 ; i++) {
            c2 = c2 * 10;
        }
        var Num1, Num2;
        var fixfloat = 0;
        fixfloat = b1 > b2 ? b1 : b2;
        if (b1 != 0) {
            Num1 = parseFloat(a1[0], 10);
            b1 = parseFloat("0." + a1[1]);
            if (Num1 < 0)
                b1 = b1 * -1
        } else {
            Num1 = parseInt(pNum1, 10);
        }
        if (b2 != 0) {
            Num2 = parseFloat(a2[0]);
            b2 = parseFloat("0." + a2[1]);
            if (Num2 < 0)
                b2 = b2 * -1
        } else {
            Num2 = parseInt(pNum2, 10);
        }

        switch (pCal) {
            case "+": var z = parseFloat((Num1 + Num2)) + parseFloat((b1 + b2).toFixed(fixfloat)); break;
            case "-": var z = parseFloat((Num1 - Num2)) + parseFloat((b1 - b2).toFixed(fixfloat)); break;
            case "*": var z = (Math.round(pNum1 * c1) * Math.round(pNum2 * c2)) / (c1 * c2); break;
            case "/": var z = (Math.round(pNum1 * c1) / Math.round(pNum2 * c2)) / c1 * c2; break;
        }
        return z + "";
    }
    else {
        return pNum1;
    }
}

function getDateDiff(date1, date2) {
    var arrDate1 = date1.split(".");
    var getDate1 = new Date(parseInt(arrDate1[2]), parseInt(arrDate1[1]) - 1, parseInt(arrDate1[0]));
    var arrDate2 = date2.split(".");
    var getDate2 = new Date(parseInt(arrDate2[2]), parseInt(arrDate2[1]) - 1, parseInt(arrDate2[0]));

    var getDiffTime = getDate1.getTime() - getDate2.getTime();

    return Math.floor(getDiffTime / (1000 * 60 * 60 * 24));
}


var SetClipBoardOnlyValue = function (obj) {
    obj.select();
    document.execCommand('Copy');

    AlertMsg("Window Alert", "Copy complete.")
}