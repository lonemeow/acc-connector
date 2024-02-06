function abort(reason) {
    alert(reason);
    throw 1;
}

function redirect() {
    const link = document.querySelector('a');
    const urlParams = new URLSearchParams(window.location.search);
    const target = urlParams.get('target');
    if (target) {
        link.textContent = `Add ${target}`;
        link.href = target;
    } else {
        const hostname = urlParams.get('hostname') ?? abort('Hostname must be set');
        const port = urlParams.get('port') ?? abort('Port must be set');
        const url = new URL(`acc-connect://${hostname}:${port}/`);

        const name = urlParams.get('name');
        const persistent = urlParams.get('persistent') == 'true';

        var linkText;
        if (name) {
            url.searchParams.set('name', name);
            linkText = `${name} (${hostname}:${port})`;
        } else {
            linkText = `${hostname}:${port}`;
        }
        if (persistent) {
            url.searchParams.set('persistent', persistent);
            linkText = '\u2605' + linkText;
        }
        link.textContent = `Add ${linkText}`;
        link.href = url.href;
    }
    link.click();
}

window.onload = redirect;
