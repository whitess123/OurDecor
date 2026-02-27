document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('materials-list')) {
        loadProductMaterials();
    }
});

async function loadProductMaterials() {
    const urlParams = new URLSearchParams(window.location.search);
    const productId = urlParams.get('id');

    if (!productId) {
        showError('ID продукта не указан');
        return;
    }

    try {
        const product = await apiRequest(`/products/${productId}`);
        displayProductInfo(product);

        const materials = await apiRequest(`/products/${productId}/materials`);
        displayMaterials(materials);

        calculateTotalCost(materials);

    } catch (error) {
        showError('Ошибка загрузки материалов: ' + error.message);
    }
}

function displayProductInfo(product) {
    const infoDiv = document.getElementById('product-info');
    if (!infoDiv) return;

    document.getElementById('product-title').textContent = `Материалы: ${product.name}`;

    infoDiv.innerHTML = `
        <h2>${product.name}</h2>
        <p><strong>Артикул:</strong> ${product.article || ''}</p>
        <p><strong>Тип:</strong> ${product.productType || ''}</p>
        <p><strong>Мин. стоимость:</strong> ${formatPrice(product.minPartnerPrice)} ₽</p>
        <p><strong>Ширина рулона:</strong> ${product.rollWidth?.toFixed(2) || '0.00'} м</p>
    `;
}

function displayMaterials(materials) {
    const tbody = document.getElementById('materials-list');

    if (!materials || materials.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="loading">Материалы не найдены</td></tr>';
        return;
    }

    tbody.innerHTML = materials.map(material => `
        <tr>
            <td>${material.materialName || ''}</td>
            <td>${material.quantityRequired?.toFixed(2) || '0.00'}</td>
            <td>${material.unitOfMeasure || ''}</td>
            <td>${formatPrice(material.unitPrice)} ₽</td>
            <td>${formatPrice(material.totalPrice)} ₽</td>
        </tr>
    `).join('');
}

function calculateTotalCost(materials) {
    const total = materials.reduce((sum, m) => sum + (m.totalPrice || 0), 0);
    const totalElement = document.getElementById('total-cost');
    if (totalElement) {
        totalElement.innerHTML = `<strong>${formatPrice(total)} ₽</strong>`;
    }
}

function formatPrice(price) {
    return price ? price.toFixed(2) : '0.00';
}

function showError(message) {
    const errorDiv = document.getElementById('error-message');
    if (errorDiv) {
        errorDiv.textContent = message;
        errorDiv.style.display = 'block';
    } else {
        alert(message);
    }
}