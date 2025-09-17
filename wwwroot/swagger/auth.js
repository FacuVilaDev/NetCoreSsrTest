(function () {
    const LS = 'swaggerBearerRaw';
    function getCookie(n) { const v = document.cookie.split('; ').find(r => r.startsWith(n + '=')); return v ? decodeURIComponent(v.split('=')[1]) : null; }
    function strip(s) { if (!s) return s; return s.toLowerCase().startsWith('bearer ') ? s.slice(7) : s; }
    function load() { return strip(getCookie('swagger_token')) || localStorage.getItem(LS) || ''; }
    function save(t) { try { localStorage.setItem(LS, t); } catch (e) { } document.cookie = 'swagger_token=' + encodeURIComponent('Bearer ' + t) + '; Path=/swagger; SameSite=Lax'; }
    function tryPreauth() {
        const t = load();
        if (!t) return;
        if (window.ui && typeof window.ui.preauthorizeApiKey === 'function') { window.ui.preauthorizeApiKey('Bearer', 'Bearer ' + t); }
    }
    window.addEventListener('load', () => {
        tryPreauth();
        const orig = window.fetch.bind(window);
        window.fetch = function (input, init) {
            init = init || {}; init.headers = new Headers(init.headers || {});
            const isLogin = typeof input === 'string' && /\/auth\/login\b/.test(input);
            if (!isLogin) {
                const tok = load();
                if (tok && !init.headers.has('Authorization')) init.headers.set('Authorization', 'Bearer ' + tok);
            }
            return orig(input, init).then(async resp => {
                try {
                    if (isLogin && resp.ok) {
                        const ct = (resp.headers.get('content-type') || '').toLowerCase();
                        if (ct.includes('application/json')) {
                            const data = await resp.clone().json();
                            const tok = strip(data.token || data.Token || data.access_token || '');
                            if (tok) { save(tok); tryPreauth(); }
                        } else {
                            const h = strip(resp.headers.get('x-auth-token') || '');
                            if (h) { save(h); tryPreauth(); }
                        }
                    }
                } catch (e) { }
                return resp;
            });
        };
    });
})();
