// Function to set a cookie with the JWT token
function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/; SameSite=Strict";
}

// Function to attach the token to all requests
function attachTokenToRequest() {
    const token = localStorage.getItem('jwt_token');
    if (token) {
        const originalFetch = window.fetch;
        window.fetch = function(url, options = {}) {
            options.headers = {
                ...options.headers,
                'Authorization': `Bearer ${token}`
            };
            return originalFetch(url, options);
        };

        // Also attach token to regular XHR requests
        const originalXHROpen = XMLHttpRequest.prototype.open;
        XMLHttpRequest.prototype.open = function() {
            const result = originalXHROpen.apply(this, arguments);
            this.setRequestHeader('Authorization', `Bearer ${token}`);
            return result;
        };
    }
}

// Function to handle successful login
function handleLoginSuccess(data) {
    if (!data.token) {
        console.error('No token received in login response');
        return;
    }
    
    // Store token in both localStorage and cookie
    localStorage.setItem('jwt_token', data.token);
    setCookie('jwt_token', data.token, 7);
    
    // Attach token to future requests
    attachTokenToRequest();

    // Log success
    console.log('Login successful:', {
        userType: data.userType,
        username: data.username,
        tokenStored: !!data.token
    });

    // Redirect with token in header
    const redirectUrl = `/${data.userType}/Dashboard`;
    
    // Use fetch for the redirect to ensure token is included
    fetch(redirectUrl, {
        headers: {
            'Authorization': `Bearer ${data.token}`
        }
    })
    .then(response => {
        if (response.ok) {
            window.location.href = redirectUrl;
        } else {
            console.error('Redirect failed:', response.status);
            throw new Error('Redirect failed');
        }
    })
    .catch(error => {
        console.error('Error during redirect:', error);
    });
}

// Initialize token attachment on page load
document.addEventListener('DOMContentLoaded', function() {
    attachTokenToRequest();
});

// Login form submission
document.getElementById('loginForm')?.addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    
    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            handleLoginSuccess(data);
        } else {
            const alertDiv = document.getElementById('loginAlert');
            alertDiv.textContent = data.message || 'Login failed';
            alertDiv.classList.remove('d-none');
        }
    } catch (error) {
        console.error('Login error:', error);
        const alertDiv = document.getElementById('loginAlert');
        alertDiv.textContent = `Error: ${error.message}`;
        alertDiv.classList.remove('d-none');
    }
});

document.getElementById('logoutForm')?.addEventListener('submit', function(e) {
    e.preventDefault();
    
    // Clear stored tokens
    localStorage.removeItem('jwt_token');
    document.cookie = 'jwt_token=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    
    // Redirect to home page
    window.location.href = '/';
});
