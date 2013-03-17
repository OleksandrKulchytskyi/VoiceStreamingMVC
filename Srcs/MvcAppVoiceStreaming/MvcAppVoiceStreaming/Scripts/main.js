if (Modernizr.audio) {

	var rec = null;
	var recGuid = undefined;
	var intervalKey;

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
		$.ajax({
			type: "POST",
			url: "/api/VoiceReceiver/Start?flag=start",
			//headers: { "UserId": uid, "AuthToken": "11111" },
			success: function (jsonStr) {
				console.log(jsonStr);
				recGuid = jsonStr;
				$('#message').val("Record id is: " + jsonStr);
				rec.record();
				console.log("Recording is started.");
				intervalKey = setInterval(function () {
					rec.exportWAV(function (blob) {
						rec.clear();
						console.log(blob);//ws.send(blob);
						sendVoiceData();
					});
				}, 3000);
			}
		}).fail(function () {
			$('#message').val('Error in loading...');
		});

		//ws.send("start");
		//$("#message").val("Click export to stop recording and analyze the input");
		// export a wav every second, so we can send it using websockets		
	});

	function sendVoiceData() {
		$.ajax({
			type: "POST",
			url: "/api/VoiceReceiver/Receive",
			headers: { "recordId": recGuid },
			success: function (jsonStr) {
				console.log("Data was sent.");
			}
		}).fail(function () {
			console.log("Data send eas fail.");
		});
	}

	$('#export').click(function () {
		recGuid = undefined;
		// first send the stop command
		rec.stop();
		//ws.send("stop");
		clearInterval(intervalKey);
		//ws.send("analyze");
		$("#message").val("Recording is stopped.");
	});
}