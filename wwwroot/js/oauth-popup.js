window.oauthPopup = {
    open: function (url, name, width, height, dotNetRef) {
        const left = (screen.width / 2) - (width / 2);
        const top = (screen.height / 2) - (height / 2);

        const popup = window.open(
            url,
            name,
            `width=${width},height=${height},top=${top},left=${left}`
        );

        const listener = (event) => {
            if (event.origin !== window.location.origin) return;

            if (event.data?.type === "oauth") {
                dotNetRef.invokeMethodAsync("ReceiveOAuthCode", event.data.code);
                window.removeEventListener("message", listener);
                popup.close();
            }
        };

        window.addEventListener("message", listener);
    }
};
