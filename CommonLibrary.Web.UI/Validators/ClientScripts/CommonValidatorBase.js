function OverrideValidatorUpdateDisplay(val) {
	if (val.displayType == "fade") {
		FadeValidatorUpdateDisplay(val);
	} else {
		ASPValidatorUpdateDisplay(val);
	}
}

function FadeValidatorUpdateDisplay(val) {
	if (!val.isvalid) {
		YAHOO.util.Dom.setStyle(val, 'opacity', 0);
		YAHOO.util.Dom.setStyle(val, 'visibility', 'visible');
		YAHOO.util.Dom.setStyle(val, 'display', '');
		val.state = 'error';
		var anim = new YAHOO.util.Anim(val, { opacity: { from: 0, to: 1 } });
		anim.animate();
	} else if (val.state && val.state == 'error') {
		val.state = 'valid';
		var anim = new YAHOO.util.Anim(val, { opacity: { from: 1, to: 0 } });
		anim.duration = 0.5;
		anim.animate();
	}
}