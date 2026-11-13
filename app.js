function createCell(fieldName, model) {
    const cell = document.createElement('td');
    cell.textContent = model[fieldName];
    return cell;
}

function renderTable(data) {
    const tableBody = document.getElementById('postings');
    const columns = document.querySelectorAll('#table-header th');
    tableBody.replaceChildren();

    for (const posting of data) {
        const row = document.createElement('tr');
        for (const column of columns) {
            const fieldName = column.getAttribute('x-column');
            row.appendChild(createCell(fieldName, posting));
        }
        tableBody.appendChild(row);
    }
}

async function refresh() {
    const status = document.getElementById('status');
    const button = document.getElementById('refresh');
    button.disabled = true;
    status.textContent = 'Завантаження відправлень…';
    try {
        const data = await loadData();
        renderTable(data);
        status.textContent = data.length ? 'Відправлень: ' + data.length : 'Відправлень немає.';
    } catch (error) {
        status.textContent = 'Не вдалося завантажити дані. ' + error.message;
    } finally {
        button.disabled = false;
    }
}

document.getElementById('refresh').addEventListener('click', refresh);
refresh();
