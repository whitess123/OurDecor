const API_BASE_URL = 'http://localhost:5118/api';

async function apiRequest(endpoint, method = 'GET', data = null) {
    const options = {
        method,
        headers: {
            'Content-Type': 'application/json'
        }
    };

    if (data) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, options);
        const result = await response.json();

        if (!response.ok) {
            throw { status: response.status, message: result.message || 'Ошибка запроса' };
        }

        return result;
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}