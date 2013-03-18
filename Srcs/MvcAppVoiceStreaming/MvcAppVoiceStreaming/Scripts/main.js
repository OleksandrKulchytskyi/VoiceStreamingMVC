if (Modernizr.audio) {

	function hasGetUserMedia() {
		// Note: Opera is unprefixed.
		return !!(navigator.getUserMedia || navigator.webkitGetUserMedia ||
				  navigator.mozGetUserMedia || navigator.msGetUserMedia);
	}

	var rec = undefined;
	var recGuid = undefined;
	var intervalKey = undefined;

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

		$("#record").attr("disabled", "disabled");
		$("#export").removeAttr("disabled");

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
						//console.log(blob); ws.send(blob);
						sendVoiceData(blob);
						
					});
				}, 1500);
			}
		}).fail(function () {
			$('#message').val('Error in loading...');
		});
	});

	function sendVoiceData(blobData) {
		console.log("sendVoiceData");
		console.log(blobData);
		if (blobData != undefined) {
			$.ajax({
				cache: false,
				type: "POST",
				url: "/api/VoiceReceiver/Receive",
				headers: { "recordId": recGuid }, //,"Content-Type": "application/octet-stream" },
				data: blobData,
				processData: false,
				contentType: false,
				success: function (reponse) {
					console.log("Data has been successfully sent to backend.");
				}
			}).fail(function () {
				console.log("Data send was fail.");
			});
		}
	}

	$('#export').click(function () {

		$("#export").attr("disabled", "disabled");
		$("#record").removeAttr("disabled");

		rec.stop();
		clearInterval(intervalKey);
		// first send the stop command
		$.ajax({
			cache: false,
			type: "GET",
			url: "/api/VoiceReceiver/Stop",
			headers: { "recordId": recGuid },
			success: function (reponse) {
				console.log("Stop operation has been successfully completed.");
			}
		}).fail(function () {
			console.log("Stop was fail.");
		});

		recGuid = undefined;
		//ws.send("analyze");
		$("#message").val("Recording is stopped.");
	});
}

function update(stream) {
	document.querySelector('audio').src = stream.url;
}