//#region Globals
var tdata;
var selectedMP;
var selectedSSP;
//#endregion Globals

//#region Onload
$('#datetimepicker3').datetimepicker({
    format: 'mm:ss'
});
//#endregion Onload

//#region Call Renderings
RenderGraph();
RenderSkeleton_MP();
RenderSkeleton_SP();
RenderResearch();
RenderConnection();
RenderTransition();
RenderSlides();
RenderCards();
//#endregion Call Renderings

//#region Radio Button Deselect Event to Autosave
setupDeselectEvent();
function setupDeselectEvent() {
    var selected = {};
    $('input[type="radio"]').on('click', function () {
        if (this.name in selected && this != selected[this.name])
            $(selected[this.name]).trigger("deselect");
        selected[this.name] = this;
    }).filter(':checked').each(function () {
        selected[this.name] = this;
    });
}
$(".radio").on("deselect", function (data) {
    var t = this;
    if (t.name == "strats") {
        jsAutoSaveBestStrat(t.id, false);
    } else {
        jsAutoSave(null, t.id, false);
    }
});
//#endregion - Radio Button Deselect Event to Autosave

//#region AutoSave Functions
function jsAutoSave(ssid, choiceID, answer) {
    var url = "/Speeches/AutoSave";
    $.post(url, { questionChoiceID: choiceID, freeText: answer }, function (data) {
        $("#msg").html(data);

        //exception logic for stage dates
        if (choiceID == 6) {
            var today = new Date();
            var enteredDate = new Date(answer);
            if (enteredDate < today) {
                alert("The date you entered is in the past. Please select a date in the future");
            }
            setStageDates(answer);
        }

        //exception logic for Evaluation step
        if (ssid == 40) {
            if (answer == 0) {
                alert('Credibility Warning!  Credible sources include an author’s name, current research, avoids bias, builds upon other credible ideas in the field (so it features a Reference page or footnotes) and is published by a credible third party.  You have indicated that this source does not meet this standard.  You should consider finding a more credible source, a librarian can help.');
            }
        }
        CheckSubStepCompletion(ssid);
    })
}
function jsAutoSaveBestStrat(id, checked) {
    var url = "/Speeches/AutoSaveBestStrat";
    $.post(url, { SpeechStrategyID: id, freeText: checked }, function (data) {
        CheckSubStepCompletion(18);
        RenderAfterMP_SP_Change();
    })
}
//#endregion AutoSave Functions

//#region Step & SubStep Checking Functions
function CheckSubStepCompletion(substepID) {
    $.post("/Speeches/CheckIfSubStepCompleted", { SubStepID: substepID }, function (data) {
        //alert(data);
        if (data == 'True') {
            checkNode(getNodeIDFromSubStepID(substepID));
            CheckStepCompletion(getNodeIDFromSubStepID(substepID));
        } else {
            uncheckNode(getNodeIDFromSubStepID(substepID));
            CheckStepCompletion(getNodeIDFromSubStepID(substepID));
        }
        disableFutureNodes();
    });
}
function CheckStepCompletion(subNodeID) {
    var _allChecked = true;
    //check this node
    var thisNode = $('#tree').treeview('getNode', subNodeID);
    var checked = thisNode.state.checked;
    if (checked == false) {
        _allChecked = false;
    }
    //check siblings
    var array = $('#tree').treeview('getSiblings', subNodeID);
    $.each(array, function (index, value) {
        var checked = value.state.checked;
        if (checked == false) {
            _allChecked = false;
        }
    });

    var parentNodeID = thisNode.parentId;
    if (_allChecked) {
        checkNode(parentNodeID);
    } else {
        uncheckNode(parentNodeID);
    }
}
//#endregion Step & SubStep Checking Functions

//#region Date Functions
function addDays(date, days) {
    var newDate = new Date();
    newDate.setTime(date.getTime() + days * 86400000);

    return newDate;
}
function getFormattedDate(date) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    return month + '/' + day + '/' + year;
}
//#endregion Date Functions

//#region Spinner Functions
function ShowSpinner() {
    $(".loading").show();
}
function HideSpinner() {
    $(".loading").hide();
}
//#endregion Spinner Functions

//#region Partial View Render Functions
function RenderGraph() {
    //ShowSpinner();
    $.post("/Speeches/RenderGraph", null, function (data) {
        $("#Graph")[0].innerHTML = data;
        HideSpinner();
    });
}

function RenderSkeleton_MP() {
    $.post("/Speeches/RenderSkeleton_MP", null, function (data) {
        $("#Skeleton_MP")[0].innerHTML = data;
        CheckSubStepCompletion(20);
    });
}

function RenderSkeleton_SP() {
    $.post("/Speeches/RenderSkeleton_SP", null, function (data) {
        $("#Skeleton_SP")[0].innerHTML = data;
    });
}

function RenderResearch() {
    $.post("/Speeches/RenderResearch", null, function (data) {
        $("#Research")[0].innerHTML = data;
    });
}

function RenderConnection() {
    $.post("/Speeches/RenderConnection", null, function (data) {
        $("#Connection")[0].innerHTML = data;
    });
}

function RenderTransition() {
    $.post("/Speeches/RenderTransition", null, function (data) {
        $("#Transition")[0].innerHTML = data;

        $("#Transitions").change(function () {
            var dropdown = $(this);
            var tID = dropdown.val();
            var ddlTxt = dropdown[0].selectedOptions[0].innerText;

            var cTransTxt = $("#trans_txt").val();
            $("#trans_txt").val(ddlTxt);
        });
    });
}

function RenderSlides() {
    $.post("/Speeches/RenderSlides", null, function (data) {
        $("#Slides")[0].innerHTML = data;
        //post render
    });
}

function RenderCards() {
    $.post("/Speeches/RenderCards", null, function (data) {
        $("#Cards")[0].innerHTML = data;
        //post render
    });
}
//#endregion Partial View Render Functions

//#region Sub Step Sections
//#region Main Point Functions
function addMainPoint() {
    var mp = $("#newMainPoint")[0].value;

    $.post("/Speeches/AddMainPoint", { strMainPoint: mp }, function (data) {
        if (data == "Successfully added new main point") {
            CheckSubStepCompletion(20);
            RenderSkeleton_MP();
        }
    });
}

function addingNewMP() {
    var nmp = $("#newMainPoint");
    var txt = nmp[0].value;
    var len = txt.length;
    if (len > 0) {
        $("#newMainPointBtn").show(200);
    } else {
        $("#newMainPointBtn").hide(200);
    }
}

function updateMainPointText(smpID) {
    //update in speechmainpoints
    var txtBox = $('#txt_' + smpID);
    var value = txtBox[0].value;

    $.post("/Speeches/UpdateSpeechMainPointText", { speechMainPointID: smpID, val: value }, function () {
        RenderSkeleton_MP();
        RenderAfterMP_SP_Change();
        RenderSkeleton_SP();
    })
}

function updateSubPointText(SpeechSubPointsID) {
    //update in speechmainpoints
    var txtBox = $('#txt_ssp_' + SpeechSubPointsID);
    var value = txtBox[0].value;

    $.post("/Speeches/UpdateSpeechSubPointText", { SpeechSubPointsID: SpeechSubPointsID, val: value }, function () {
        RenderAfterMP_SP_Change();
    })
}

function MoveSMP_up(speechMainPointID) {
    $.post("/Speeches/MoveSMP_up", { speechMainPointID: speechMainPointID }, function () {
        RenderAfterMP_SP_Change();
    });
}

function MoveSMP_down(speechMainPointID) {
    $.post("/Speeches/MoveSMP_down", { speechMainPointID: speechMainPointID }, function () {
        RenderAfterMP_SP_Change();
    });
}

function MoveSSP(SpeechSubPointsID, Direction) {
    $.post("/Speeches/MoveSSP", { SpeechSubPointsID: SpeechSubPointsID, direction: Direction }, function () {
        RenderAfterMP_SP_Change();
    });
}

function DeleteSMP(speechMainPointID) {
    $.post("/Speeches/DeleteSMP", { speechMainPointID: speechMainPointID }, function () {
        CheckSubStepCompletion(20);
        RenderAfterMP_SP_Change();
    });
}

function SetSelectedMP(SpeechMainPointID) {
    selectedMP = SpeechMainPointID;
}

function SetSelectedSSP(SpeechSubPointID) {
    selectedSSP = SpeechSubPointID;
}

function AddSubPoint() {
    var myRadio = $('input[name=strat]');
    var checked = myRadio.filter(':checked');
    var id = checked[0].id;

    var customSPtxt = $("#txtCustomSubpoint").val();
    if (customSPtxt != "") {
        $.post("/Speeches/AddSubpointCustom", { customSPtxt: customSPtxt, SpeechMainPointID: selectedMP }, function (data) {
            CheckSubStepCompletion(19);
            RenderAfterMP_SP_Change();
        });
    } else {
        $.post("/Speeches/AddSubPoint", { StrategyID: id, SpeechMainPointID: selectedMP }, function (data) {
            CheckSubStepCompletion(19);
            RenderAfterMP_SP_Change();
        });
    }
}
//#endregion Main Point Functions

//#region SubPoints
function DeleteSSP(SpeechSubPointsID) {
    $.post("/Speeches/DeleteSSP", { SpeechSubPointsID: SpeechSubPointsID }, function () {
        CheckSubStepCompletion(19);
        RenderAfterMP_SP_Change();
    });
}
//#endregion SubPoints

//#region Connections
function EditConnectionSetupModal(sspConnID, cTypeID) {
    var lblTxt = $("#lblTxt_" + sspConnID + "");
    var txt = lblTxt[0].innerText;
    var txtEdit = $("#txtArea_ConnectionEdit").val(txt);
    $("#r_" + cTypeID).attr('checked', true);
    $("#editConn_sspConnID").val(sspConnID);
}

function SaveEditConnection(sConnID) {
    var myRadio = $('input[name=connection_RadiosEdit]');
    var checked = myRadio.filter(':checked');
    var cTypeID = checked[0].id.replace("r_", "");
    var sConnID = $("#editConn_sspConnID").val();
    var cTxt = $("#txtArea_ConnectionEdit").val();
    $.post("/Speeches/EditConnection", { sspConnID: sConnID, connTypeID: cTypeID, txt: cTxt }, function (data) {
        RenderConnection();
    });
}

function DeleteConnection(sConnID) {
    $.post("/Speeches/DeleteConnection", { id: sConnID }, function () {
        CheckSubStepCompletion(22);
        RenderConnection();
    });
}

function AddConnection() {
    var myRadio = $('input[name=connection_Radios]');
    var checked = myRadio.filter(':checked');
    var id = checked[0].id;

    var cnnText = $("#txtArea_Connection").val();
    $.post("/Speeches/AddConnection", { SpeechSubPointID: selectedSSP, ConnectionTypeID: id, Txt: cnnText }, function (data) {
        CheckSubStepCompletion(22);
        RenderAfterResearch_Change();
    });
}
//#endregion Connections

//#region Transitions
function saveSSP_DDL_Transition() {
    var id = $("#Transitions").val();
    var txt = $("#trans_txt")[0].value;
    txt = txt.replace("...", "");
    $.post("/Speeches/saveSSP_DDL_Transition", { SpeechSubPointID: selectedSSP, TransitionID: id, TransitionText: txt }, function (data) {
        CheckSubStepCompletion(23);
        RenderTransition();
    });
}

function EditTransition(SpeechSubPointsID, txtBoxID) {
    var editTxt = txtBoxID.value;

    $.post("/Speeches/EditTransition", { SpeechSubPointsID: SpeechSubPointsID, txt: editTxt }, function (data) {
        CheckSubStepCompletion(23);
        RenderTransition();
    });
}

function setTrans_Txt(ssid) {
    selectedSSP = SpeechSubPointID;
    $("#trans_txt").val(txt);
}
//#endregion Transitions

//#region Research
function recordResearchSummary(SpeechSubPointsID, summary) {
    //alert('SSPID:' + SpeechSubPointsID + ' Summary:' + summary);
    $.post("/Speeches/SaveSourceSummary", { SpeechSubPointsID: SpeechSubPointsID, summary: summary }, function (data) {
        //RenderAfterResearch_Change();
        CheckSubStepCompletion(21);
    });
}

function DeleteSSPR(SpeechSubpointResearchID) {
    $.post("/Speeches/DeleteSSPR", { SpeechSubpointResearchID: SpeechSubpointResearchID }, function () {
        RenderAfterResearch_Change();
    });
}

function RenderAfterMP_SP_Change() {
    RenderSkeleton_MP();
    RenderSkeleton_SP();
    RenderResearch();
    RenderConnection();
    RenderTransition();
}

function RenderAfterResearch_Change() {
    RenderResearch();
    RenderConnection();
    RenderTransition();
}

function AddSourceToSubPoint() {
    var myRadio = $('input[name=radioSources]');
    var checked = myRadio.filter(':checked');
    var id = checked[0].id;

    $.post("/Speeches/AddSourceToSubPoint", { SpeechSubPointID: selectedSSP, SourceID: id }, function (data) {
        RenderResearch();
        RenderConnection();
    });
}
//#endregion Research

//#region Introduction
function LoadIntroPreview() {
    var prepend = "Let's discuss: ";
    var textArea = $("#73");
    if (textArea.val().indexOf(prepend) == -1) {
        //Text Area - blank, populate
        $.post("/Speeches/getSpeechMainPoints_Str", {}, function (data) {
            if (data != "") {
                var str = prepend + data;
                textArea.val(str);
                jsAutoSave(34, 73, str);
            }
        });
    }
}
function LoadIntroStateEthos() {
    var intro_origin = $("#54").val();
    var target = $("#71");
    var prepend = "So today, I would like to discuss with you how to ";
    if (target.val().indexOf(prepend) == -1) {
        var str = prepend + intro_origin
        target.val(str);
        jsAutoSave(26, 71, str);
    }
}
//#endregion Introduction
//#region Conclusion
function LoadConc_Attn() {
    var intro_origin = $("#69").val();
    var target = $("#78");
    if (target.val() == "") {
        var str = intro_origin
        target.val(str);
        jsAutoSave(31, 78, str);
    }
}
function LoadConc_Relate() {
    var intro_origin = $("#70").val();
    var target = $("#77");
    var prepend = "And, in so doing, you can ";
    if (target.val().indexOf(prepend) == -1) {
        var str = prepend + intro_origin
        target.val(str);
        jsAutoSave(30, 77, str);
    }
}
function LoadConc_State() {
    var intro_origin = $("#54").val();
    var target = $("#76");
    var prepend = "Thus by incorporating these skills in our lives we can ";
    if (target.val().indexOf(prepend) == -1) {
        var str = prepend + intro_origin
        target.val(str);
        jsAutoSave(29, 76, str);
    }
}
function LoadConc_Support() {
    var intro_origin = $("#72").val();
    var target = $("#75");
    var prepend = "Remember how ";
    if (target.val().indexOf(prepend) == -1) {
        var str = prepend + intro_origin
        target.val(str);
        jsAutoSave(28, 75, str);
    }
}

function LoadConc_Review() {
    var prepend = "To review, today we discussed ";
    var textArea = $("#74");
    if (textArea.val().indexOf(prepend) == -1) {
        //Text Area - blank, populate
        $.post("/Speeches/getSpeechMainPoints_Str", {}, function (data) {
            if (data != "") {
                var str = prepend + data;
                textArea.val(str);
                jsAutoSave(35, 74, str);
            }
        });
    }
}

//#endregion Conclusion
//#endregion Sub Step Sections

loadDates();
function loadDates() {
    $.post("/Speeches/GetAnswer", { questionChoiceID: 8 }, function (data) {
        $("#8").val(data);
    });

    $.post("/Speeches/GetAnswer", { questionChoiceID: 9 }, function (data) {
        $("#9").val(data);
    });

    $.post("/Speeches/GetAnswer", { questionChoiceID: 10 }, function (data) {
        $("#10").val(data);
    });

    $.post("/Speeches/GetAnswer", { questionChoiceID: 82 }, function (data) {
        $("#82").val(data);
    });

    $.post("/Speeches/GetAnswer", { questionChoiceID: 83 }, function (data) {
        $("#83").val(data);
    });

    $.post("/Speeches/GetAnswer", { questionChoiceID: 84 }, function (data) {
        $("#84").val(data);
    });
}

//#region Keep Alive
SetupSessionUpdater('/Speeches/KeepSessionAlive');

var keepSessionAlive = false;
var keepSessionAliveUrl = null;

function SetupSessionUpdater(actionUrl) {
    keepSessionAliveUrl = actionUrl;
    var container = $("body");
    container.mousemove(function () {
        keepSessionAlive = true;
    });
    container.keydown(function () { keepSessionAlive = true; });
    CheckToKeepSessionAlive();
}

function CheckToKeepSessionAlive() {
    setTimeout("KeepSessionAlive()", 10000);
}

function KeepSessionAlive() {
    if (1 == 1) {
        $.ajax({
            type: "POST",
            url: '/Speeches/KeepSessionAlive',
            success: function (data) {
                keepSessionAlive = false;
                //alert('alive!');
            }
        });
    }
    CheckToKeepSessionAlive();
}

//#endregion Keep Alive

$("#actionVerbs li").click(function () {
    var t = $(this)[0].innerText;
    var ta = $("#54")[0];
    var it = ta.value + "";
    if (it == "") {
        ta.value = t.toLowerCase();
    } else {
        ta.value = it + " " + t.toLowerCase();
    }
    $("#actionVerbsModal").modal('hide');
});

//#region YouTuve Videos
function showIframe(ssid) {
    var ifr = $("#iframe_yt_" + ssid)[0];
    var hid = $("#hidden_yt_" + ssid)[0].value;
    ifr.src = hid;
}
//#endregion YouTuve Videos

//tinymce.init({
//    selector: '.tinymce',
//    height: 500,
//    plugins: [
//      'advlist autolink lists link image charmap print preview hr anchor pagebreak',
//      'searchreplace wordcount visualblocks visualchars code fullscreen',
//      'insertdatetime media nonbreaking save table contextmenu directionality',
//      'emoticons template paste textcolor colorpicker textpattern imagetools'
//    ],
//    toolbar1: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',
//    toolbar2: 'print preview media | forecolor backcolor emoticons',
//    image_advtab: true,
//    templates: [
//      { title: 'Test template 1', content: 'Test 1' },
//      { title: 'Test template 2', content: 'Test 2' }
//    ],
//    content_css: [
//      '//fast.fonts.net/cssapi/e6dc9b99-64fe-4292-ad98-6974f93cd2a2.css',
//      '//www.tinymce.com/css/codepen.min.css'
//    ]
//});