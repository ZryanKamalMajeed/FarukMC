$(function () {
	setTimeout(printMessages, 200);
});


var Messages = [];
function printMessages() {
	var msgs = Messages.msgs;
	if (msgs && msgs.length > 0)
		alert(msgs.join("\r\n"), 'Alert', Messages.priority);
}


//ALERT & CONFIRM
var modalTargets = [];

window.alert = function (message, title, priority) {
    if (String(message) !== '') {
        $("#alert_modal .modal-title").text(title);
        $("#alert_modal .modal-body").html(String(message).replace(/\r\n/g, "<br/>"));
        alertPriority(priority ? priority : 0);
        $("#alert_modal").modal("show");
    }
    return false;
};

window.alertPromise = function (message, title, priority) {
    if (String(message) !== '') {
        $("#alert_modal .modal-title").text(title);
        $("#alert_modal .modal-body").html(String(message).replace(/\r\n/g, "<br/>"));
        alertPriority(priority ? priority : 0);
        $("#alert_modal").modal("show");
    }
    return false;
};

var confirmNextAction = null;
//window.confirm = function (message, title) {
    
//    var s = Confirm(event.target, message, title);
    
//    return s;
//};

function alertPriority(priority) {
	switch (priority) {
		case 0: //normal
			$("#alert_modal .modal-header, #alert_modal .modal-footer button").removeClass("bg-danger").addClass("bg-blue");
			break;
        case 1: //high
			$("#alert_modal .modal-header, #alert_modal .modal-footer button").removeClass("bg-blue").addClass("bg-danger");
            break;
        case 2: //Continue
            $("#alert_modal .modal-footer button").html("Continue");
            $("#alert_modal .modal-header, #alert_modal .modal-footer button").removeClass("bg-blue").addClass("bg-danger");
            
            break;
	}

}

function Confirm(target, message, title, okText, cancelText) {
    // console.log(typeof target, typeof target === 'function');
    if (typeof target === 'function' || !$(target).is(".btn-oneclick"))
        modalTargets.push(getConfirmTarget(target));

	if (message) {
		$("#confirm_modal .modal-body").html(message.replace(/\r\n/g, "<br/>"));
        $('#confirm_modal').modal('show');
	}
    if (title)
        $("#confirm_modal .modal-title").text(title);
    if (okText)
        $("#confirm_modal .modal-footer button:first").text(okText);
    if (cancelText)
        $("#confirm_modal .modal-footer button:last").text(cancelText);
	return false;
}

/* if not an input or anchor, find nearest parent that is */
function getConfirmTarget(target) {
    if (typeof target === 'function') {
        return target;
    }
    return $(target).closest("a,input,button")[0];
}

var postPressAction = null;
function ConfirmOK() { modalOK('#confirm_modal'); if (confirmNextAction) confirmNextAction(); }
function modalOK(targetElem) {
    $(targetElem).modal('hide');
    var target = modalTargets.pop();
    if (typeof target === 'function') {
        target();
    } else if (target) {
        if ($(target).is('.btn-oneclick')) {
            //probably not needed
            $(target).addClass("disabled");
            $(target).attr("disabled", "disabled");
        }
        if ($(target).is('[href]'))
            window.location = $(target).attr('href');
        else if ($(target).is('[data-ok]'))
            eval($(target).attr('data-ok'));

        var $target = $(target);
        if ($target.data('fevent')) {
            FEvent.trigger($target.data('fevent'), $target.data('fval'));
        }
    }
    if (postPressAction) {
        postPressAction();
    }
}

function alertOK() { modalCancel('#alert_modal', 'data-ok'); }

function ConfirmCancel() { modalCancel('#confirm_modal', 'data-cancel'); }
function modalCancel(targetElem, runAttr) {
    $(targetElem).modal('hide');
    var target = modalTargets.pop();
    if (typeof target !== 'function' && target) {
        if ($(target).is('.btn-oneclick')) {
            $(target).removeClass("disabled");
            $(target).removeAttr("disabled", "disabled");
        }
        if ($(target).is('[' + runAttr + ']'))
            eval($(target).attr(runAttr));
    }
    if (postPressAction) {
        postPressAction();
    }
}

//-----------------------------------------------------------