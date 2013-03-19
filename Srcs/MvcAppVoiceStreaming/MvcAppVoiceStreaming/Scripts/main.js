﻿if (Modernizr.audio) {

	function hasGetUserMedia() {
		// Note: Opera is unprefixed.
		return !!(navigator.getUserMedia || navigator.webkitGetUserMedia ||
				  navigator.mozGetUserMedia || navigator.msGetUserMedia);
	}

	var rec = undefined;
	var recGuid = undefined;
	var timerInvocation = 0;

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

	$("#download").attr("disabled", "disabled");
	$("#export").attr("disabled", "disabled");

	$('#record').click(function () {
		$("#download").attr("disabled", "disabled");
		$("#record").attr("disabled", "disabled");
		$("#export").removeAttr("disabled");

		$.ajax({
			type: "POST",
			url: "/api/VoiceReceiver/Start?flag=start",
			success: function (jsonStr) {
				console.log(jsonStr);
				recGuid = jsonStr;
				$('#message').val("Record id is: " + jsonStr);
				rec.record();
				console.log("Recording is started.");
			}
		}).fail(function () {
			$('#message').val('Error in loading...');
		});
	});

	$('#export').click(function () {
		rec.stop();

		$("#export").attr("disabled", "disabled");

		rec.exportWAV(function (blob) {
			rec.clear();
			sendVoiceData(blob);
		});
		$("#message").val("Recording is stopped.");
	});

	function sendVoiceData(blobData) {
		console.log(blobData);
		if (blobData != undefined) {
			$.ajax({
				cache: false,
				type: "POST",
				url: "/api/VoiceReceiver/Receive",
				headers: { "recordId": recGuid }, //,"Content-Type": "application/octet-stream" },
				data: blobData,
				processData: false,
				contentType: "audion/wav",
				success: function (reponse) {
					console.log("Record has been successfully sent to the backend.");
				}
			}).fail(function () {
				console.log("Error has occurred while sending record.");
			}).always(function () {
				$("#record").removeAttr("disabled");
				console.log("Processing always callback.");
				$.ajax({
					cache: false,
					type: "GET",
					url: "/api/VoiceReceiver/Stop",
					headers: { "recordId": recGuid },
					success: function (reponse) {
						console.log(recGuid);
						console.log("Stop operation has been successfully completed.");
						$("#download").removeAttr("disabled");
					}
				}).fail(function () {
					console.log("Stop was fail.");
				});
			});
		}
	}

	$('#download').click(function (e) {
		e.preventDefault();
		window.location.href = "/Home/Download?record=" + recGuid;
	});
}