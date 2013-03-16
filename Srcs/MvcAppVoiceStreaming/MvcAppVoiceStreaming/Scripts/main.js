if (Modernizr.audio) {
	var rec = null;
	navigator.getUserMedia ||
	(navigator.getUserMedia = navigator.mozGetUserMedia || navigator.webkitGetUserMedia || navigator.msGetUserMedia);

	if (navigator.getUserMedia) {
		//navigator.getUserMedia({ video: false, audio: true}, onSuccess, onError);
		navigator.webkitGetUserMedia({ audio: true }, callback);
	} else {
		alert('getUserMedia is not supported in this browser.');
	}

	function onSuccess() {
		alert('Successful!');
	}

	function onError() {
		alert('There has been a problem retrieving the streams - did you allow access?');
	}

	function callback(stream) {
		try {
			var context = new webkitAudioContext();
			var mediaStreamSource = context.createMediaStreamSource(stream);
			rec = new Recorder(mediaStreamSource);
		} catch (e) {
			alert(e.message);
		}
	}

	$('#record').click(function () {
		rec.record();
		//ws.send("start");
		$("#message").text("Click export to stop recording and analyze the input");
		// export a wav every second, so we can send it using websockets
		intervalKey = setInterval(function () {
			rec.exportWAV(function (blob) {
				rec.clear();
				console.log(blob);
				//ws.send(blob);
			});
		}, 1000);
	});

	$('#export').click(function () {
		// first send the stop command
		rec.stop();
		//ws.send("stop");
		clearInterval(intervalKey);
		//ws.send("analyze");
		$("#message").text("");
	});
}