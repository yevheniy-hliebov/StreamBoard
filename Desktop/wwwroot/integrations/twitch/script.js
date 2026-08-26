(async function () {
    const hash = window.location.hash.substring(1);
    if (!hash) {
        document.getElementById('status').innerText = 'Error';
        document.getElementById('message').innerText = 'Token not found in URL.';
        document.getElementById('loader').style.display = 'none';
        return;
    }

    const params = new URLSearchParams(hash);
    const data = {
        access_token: params.get('access_token'),
        state: params.get('state'),
        scope: params.get('scope'),
        token_type: params.get('token_type')
    };

    try {
        const response = await fetch(window.location.pathname, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            document.getElementById('status').innerText = 'Success!';
            document.getElementById('message').innerText = 'You are fully authorized. You may now close this tab.';
            document.getElementById('loader').style.display = 'none';
        } else {
            throw new Error('Server returned error');
        }
    } catch (e) {
        document.getElementById('status').innerText = 'Connection Error';
        document.getElementById('message').innerText = 'Failed to transfer data to the application.';
        document.getElementById('loader').style.display = 'none';
    }
})();