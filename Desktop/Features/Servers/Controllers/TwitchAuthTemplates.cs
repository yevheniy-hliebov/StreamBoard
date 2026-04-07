namespace StreamBoard.Features.Servers.Controllers
{
    public static class TwitchAuthTemplates
    {
        public const string AuthPageHtml = @"
<!DOCTYPE html>
<html>
<head>
    <title>Twitch Auth - StreamBoard</title>
    <meta charset='utf-8'>
    <style>
        body { font-family: sans-serif; display: flex; justify-content: center; align-items: center; height: 100vh; background: #0f0f0f; color: white; margin: 0; }
        .card { background: #18181b; padding: 2rem; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.5); text-align: center; max-width: 400px; }
        h1 { color: #a970ff; margin-bottom: 1rem; }
        p { color: #adadb8; }
        .loader { border: 4px solid #3f3f46; border-top: 4px solid #a970ff; border-radius: 50%; width: 30px; height: 30px; animation: spin 1s linear infinite; margin: 20px auto; }
        @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
    </style>
</head>
<body>
    <div class='card'>
        <h1 id='status'>Authorizing...</h1>
        <p id='message'>Retrieving data from Twitch and transferring it to the StreamBoard app.</p>
        <div id='loader' class='loader'></div>
    </div>

    <script>
        (async function() {
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
    </script>
</body>
</html>";
    }
}