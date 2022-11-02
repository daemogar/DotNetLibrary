window.DotNetLibrary = window["DotNetLibrary"] || {};
window.DotNetLibrary["Cookies"] = {
	Set: (cookie) => {
		document.cookie = cookie;
	},
	Get: (cookie) => {
		var cookies = document.cookie.split(';');

		for (var i = 0; i < cookies.length; i++) {
			let [key, value] = cookies[i].split('=');
			if (key === cookie)
				return value;
		}

		return null;
	}
};
