//#region Tree Globals
var selectedNodeID;
var parentNodeID;
var dropdownIDFull;
var fromBtnClick;
//#endregion Tree Globals

//#region Initialize Treeview
GetTreeFromDB();
function GetTreeFromDB() {
    var url = "/Speeches/GetTreeData";
    $.post(url, null, function (data) {
        var treeArray = [];
        var nid = 0;
        $.each(data, function (index, optionData) {
            var optionDataLen = optionData.length;
            var sName = optionData.Text;
            var Nodes = optionData.Nodes;
            var Idx = optionData.Idx;
            var nid = optionData.nid;

            var step = { text: sName, nodes: Nodes, idx: Idx, nid: nid };

            treeArray.push(step);
        });
        tdata = treeArray;
        $('#tree').treeview({
            color: "#428bca",
            highlightSelected: true,
            data: tdata,
            showIcon: false,
            showCheckbox: true,
            uncheckedIcon: "glyphicon glyphicon-unchecked",
            checkedIcon: "glyphicon glyphicon-check",
            onNodeSelected: function (event, node) {
                nodeSelect(event, node);
            },
            onNodeUnselected: function (event, node) {
                nodeUNselect(event, node);
            }
        });

        //post initialization configurations of treeview
        $('#tree').treeview('collapseAll', { silent: true });
        $('#tree').treeview('expandNode', [0]);
        $('#tree').treeview('selectNode', [1]);

        CheckCompletedNodes();
    });
}
//#endregion Tree Button Navigation

//#region Navigation
function Previous(tFullID) {
    var currentNodeID = selectedNodeID;
    $('#tree').treeview('selectNode', [currentNodeID - 1]);

    if (parentNodeID == undefined) {
        $('#tree').treeview('expandNode', [currentNodeID - 1]);
        $('#tree').treeview('selectNode', [currentNodeID - 2]);
    }

    fromBtnClick = true;
}
function Next(tFullID) {
    var sn = getSelectedNode();
    var snid = sn[0].nodeId;
    var snhref = sn[0].href;
    var snparentId = sn[0].parentId;
    var currentNodeID = snid;
    var parentNode = getParentNode(snid);
    var originatingStepID = parentNode.idx;

    //select next node
    var nextNodeID = currentNodeID + 1;
    var nextNode = getNode(nextNodeID);
    var nextNodeDisabled = nextNode.state.disabled;

    if (!nextNodeDisabled) {
        $('#tree').treeview('selectNode', [nextNodeID]);
        var nextNode = getSelectedNode();
        //see if 'next node' is parent node, if so then select one more down
        if (nextNode[0].parentId == undefined) {
            // next node is parent of next step
            $('#tree').treeview('expandNode', [currentNodeID + 1]);
            $('#tree').treeview('selectNode', [currentNodeID + 2]);
        }
    } else {
        bootbox.alert("Cannot proceed to next step until all previous steps are completed.");
    }

    fromBtnClick = true;

    RefreshDashBoard();
}
//#endregion Navigation

//#region Checking & Disabling
function disableFutureNodes() {
    $('#tree').treeview('enableAll', { silent: true });
    var topNode = getNode(0);
    var array = $('#tree').treeview('getSiblings', 0);
    var checked = topNode.state.checked;

    //see if top node is checked, if not disable all future nodes from next step
    if (!checked) {
        var next = array[0].nodeId;
        //disable further nodes
        for (var i = next; i < 100; i++) {
            disableNode(i);
        }
    } else {
        //first is checked, evaluate remaining
        $.each(array, function (index, value) {
            //if step node is not checked, get next step nodeid and disable remaining nodes
            var checked = value.state.checked;
            if (!checked) {
                //get next step nodeId
                try {
                    var nextStepNodeId = array[index + 1].nodeId;
                    //disable further nodes
                    for (var i = nextStepNodeId; i < 100; i++) {
                        disableNode(i);
                    }
                } catch (e) {
                }
            }
        });
    }
}
function CheckCompletedNodes() {
    $.post("/Speeches/GetAllCompletedSubSteps", null, function (data) {
        uncheckAll();
        CheckCompletedNodes_Success(data);
    });
}
function CheckCompletedNodes_Success(data) {
    $.each(data, function (index, optionData) {
        //for each completed ss, check the node box
        var sid = optionData.sid;
        var ssid = optionData.ssid;
        var ssname = optionData.ssname;
        var sname = optionData.sname;

        //get nodeid for ss, then check the box
        var nid = getNodeIDFromSubStepID(ssid);
        checkNode(nid);
        CheckStepCompletion(nid);
    });

    disableFutureNodes();
}
//#endregion Checking & Disabling

//#region  helpers - Treeview
function getParentNode(nodeID) {
    return $('#tree').treeview('getParent', nodeID);
}
function getStepIDfromDD() {
    var r = dropdownIDFull.replace("dropdown", "");
    var delim = r.indexOf("-");
    r = r.substring(0, delim);

    return r;
}
function getNodeIDFromStepID(StepID) {
    var allnodes = tdata;
    var sobj = allnodes.filter(function (sobj) {
        return sobj.idx === StepID;
    })[0];
    return sobj.nid;
}
function getNodeIDFromSubStepID(SubStepID) {
    var allnodes = tdata;
    for (var i = 0; i < allnodes.length; i++) {
        var ssobj = allnodes[i].nodes.filter(function (ssobj) {
            return ssobj.Idx === SubStepID;
        })[0];

        if (ssobj != undefined) {
            return ssobj.nid;
        }
    }
}
function getNode(nodeID) {
    var thisNode = $('#tree').treeview('getNode', nodeID);
    return thisNode;
}
function getSelectedNode() {
    var s = $('#tree').treeview('getSelected');
    return s;
}

function disableNode(nodeID) {
    $('#tree').treeview('disableNode', [nodeID, { silent: true }]);
}
function enableNode(nodeID) {
    $('#tree').treeview('enableNode', [nodeID, { silent: true }]);
}

function checkNode(nodeID) {
    $('#tree').treeview('checkNode', [nodeID]);
}
function uncheckNode(nodeID) {
    $('#tree').treeview('uncheckNode', [nodeID]);
}
function uncheckAll() {
    $('#tree').treeview('uncheckAll', { silent: true });
}

function nodeSelect(event, node) {
    $('#' + node.href + '').show(100);
    selectedNodeID = node.nodeId;
    dropdownIDFull = node.href;
    parentNodeID = node.parentId;
    slideName = node.text;

    if (parentNodeID == undefined && fromBtnClick != true) {
        //parent node/step selected
        $('#tree').treeview('expandNode', [selectedNodeID]);
        $('#tree').treeview('selectNode', [selectedNodeID + 1]);
    } else {
        //sub step selected
        switch (slideName) {
            case "Select Your Best Strategy":
                RenderBestStrategy();
                break;
            case "Outline - Main Points":
                RenderSkeleton_MP(false);
                break;
            case "Outline - Sub Points":
                RenderSkeleton_SP(false);
                break;
            case "Source":
                RenderResearch(false);
            case "Connection":
                RenderConnection(true);
                break;
            case "Transitions":
                RenderTransition();
                break;
            case "Slides":
                RenderSlides(false);
                break;
            case "Cards":
                RenderCards(false);
                break;
            case "Organizational Matrix - Graph (Review)":
                RenderGraph(true);
                break;
            case "Thesis - Support":
                LoadConc_Support();
                break;
            case "Thesis - State (Ethos)":
                LoadIntroStateEthos();
                if (parentNodeID == 30) {
                    LoadConc_State();
                }
                break;
            case "Thesis - Relate (Pathos)":
                if (parentNodeID == 30) {
                    LoadConc_Relate();
                }
                break;
            case "Attention Grabber":
                if (parentNodeID == 30) {
                    LoadConc_Attn();
                }
                break;
            case "Preview Main Points":
                LoadIntroPreview();
                break;
            case "Review Main Points":
                LoadConc_Review();
                break;
            default:
        }
    }
    fromBtnClick = false;
}
function nodeUNselect(event, node) {
    //$('#selectable-output').prepend('<p>' + node.href + ' was unselected</p>');
    $('#' + node.href + '').hide(200);
}
//#endregion Helpers - treeview