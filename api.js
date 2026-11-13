const apiUrl = 'http://localhost:5211/postings';

async function checkResponse(response) {
    if (!response.ok) {
        let message = 'Помилка сервера: ' + response.status;
        const text = await response.text();
        if (text) {
            try {
                const problem = JSON.parse(text);
                if (problem.errors) {
                    message += '. ' + Object.values(problem.errors).flat().join(' ');
                }
            } catch {
                message += '. Не вдалося виконати запит.';
            }
        }
        throw new Error(message);
    }
}

async function loadData() {
    const response = await fetch(apiUrl);
    await checkResponse(response);
    const data = await response.json();
    console.log(data);
    return data;
}

async function getPosting(id) {
    const response = await fetch(apiUrl + '/' + id);
    await checkResponse(response);
    return await response.json();
}

async function createPosting(posting) {
    const response = await fetch(apiUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(posting)
    });
    await checkResponse(response);
    return await response.json();
}

async function updatePosting(id, posting) {
    const response = await fetch(apiUrl + '/' + id, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(posting)
    });
    await checkResponse(response);
    return await response.json();
}

async function deletePosting(id) {
    const response = await fetch(apiUrl + '/' + id, { method: 'DELETE' });
    await checkResponse(response);
}

