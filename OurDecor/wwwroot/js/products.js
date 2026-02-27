document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('products-list')) {
        loadProducts();
    }

    if (document.getElementById('product-form')) {
        loadProductTypes();
        loadProductForEdit();
    }
});

async function loadProducts() {
    try {
        const products = await apiRequest('/products');
        displayProducts(products);
    } catch (error) {
        showError('Ошибка загрузки продуктов: ' + error.message);
    }
}

function displayProducts(products) {
    const tbody = document.getElementById('products-list');

    if (!products || products.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" class="loading">Нет данных</td></tr>';
        return;
    }

    tbody.innerHTML = products.map(product => `
        <tr onclick="viewProductMaterials(${product.id})">
            <td>${product.name || ''}</td>
            <td>${product.article || ''}</td>
            <td>${product.productType || ''}</td>
            <td>${formatPrice(product.minPartnerPrice)} ₽</td>
            <td>${product.rollWidth?.toFixed(2) || '0.00'} м</td>
            <td>${formatPrice(product.totalCost)} ₽</td>
            <td>
                <button class="btn btn-secondary" onclick="event.stopPropagation(); editProduct(${product.id})">Редактировать</button>
                <button class="btn btn-secondary" onclick="event.stopPropagation(); deleteProduct(${product.id})">Удалить</button>
            </td>
        </tr>
    `).join('');
}

function formatPrice(price) {
    return price ? price.toFixed(2) : '0.00';
}

function viewProductMaterials(productId) {
    window.location.href = `product-materials.html?id=${productId}`;
}

function editProduct(productId) {
    window.location.href = `product-form.html?id=${productId}`;
}

async function deleteProduct(productId) {
    if (!confirm('Вы уверены, что хотите удалить этот продукт?')) {
        return;
    }

    try {
        await apiRequest(`/products/${productId}`, 'DELETE');
        loadProducts();
        showSuccess('Продукт успешно удален');
    } catch (error) {
        showError('Ошибка при удалении: ' + error.message);
    }
}

async function loadProductTypes() {
    try {
        const types = await apiRequest('/producttypes');
        const select = document.getElementById('product-type');

        select.innerHTML = '<option value="">Выберите тип</option>' +
            types.map(type => `<option value="${type.id}">${type.name}</option>`).join('');
    } catch (error) {
        showError('Ошибка загрузки типов: ' + error.message);
    }
}

async function loadProductForEdit() {
    const urlParams = new URLSearchParams(window.location.search);
    const productId = urlParams.get('id');

    if (!productId) return;

    try {
        const product = await apiRequest(`/products/${productId}`);

        document.getElementById('form-title').textContent = 'Редактирование продукции';
        document.getElementById('product-id').value = product.id;
        document.getElementById('article').value = product.article || '';
        document.getElementById('name').value = product.name || '';
        document.getElementById('min-price').value = product.minPartnerPrice || '';
        document.getElementById('roll-width').value = product.rollWidth || '';

        const typeSelect = document.getElementById('product-type');
        await waitForSelectOptions(typeSelect);
        typeSelect.value = product.productTypeId;

    } catch (error) {
        showError('Ошибка загрузки продукта: ' + error.message);
    }
}

function waitForSelectOptions(select) {
    return new Promise(resolve => {
        if (select.options.length > 1) {
            resolve();
        } else {
            const observer = new MutationObserver(() => {
                if (select.options.length > 1) {
                    observer.disconnect();
                    resolve();
                }
            });
            observer.observe(select, { childList: true, subtree: true });
        }
    });
}

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('product-form');
    if (form) {
        form.addEventListener('submit', handleProductSubmit);
    }
});

async function handleProductSubmit(event) {
    event.preventDefault();

    const productId = document.getElementById('product-id').value;
    const productData = {
        name: document.getElementById('name').value,
        article: document.getElementById('article').value,
        productTypeId: parseInt(document.getElementById('product-type').value),
        minPartnerPrice: parseFloat(document.getElementById('min-price').value),
        rollWidth: parseFloat(document.getElementById('roll-width').value)
    };

    if (productData.minPartnerPrice < 0) {
        showError('Минимальная стоимость не может быть отрицательной');
        return;
    }
    if (productData.rollWidth < 0) {
        showError('Ширина рулона не может быть отрицательной');
        return;
    }

    try {
        if (productId) {
            await apiRequest(`/products/${productId}`, 'PUT', productData);
            showSuccess('Продукт успешно обновлен');
        } else {
            await apiRequest('/products', 'POST', productData);
            showSuccess('Продукт успешно добавлен');
        }

        setTimeout(() => {
            window.location.href = 'index.html';
        }, 1500);

    } catch (error) {
        showError(error.message || 'Ошибка при сохранении');
    }
}

function showError(message) {
    const errorDiv = document.getElementById('error-message');
    if (errorDiv) {
        errorDiv.textContent = message;
        errorDiv.style.display = 'block';
        setTimeout(() => {
            errorDiv.style.display = 'none';
        }, 5000);
    } else {
        alert(message);
    }
}

function showSuccess(message) {
    const successDiv = document.getElementById('success-message');
    if (successDiv) {
        successDiv.textContent = message;
        successDiv.style.display = 'block';
        setTimeout(() => {
            successDiv.style.display = 'none';
        }, 3000);
    }
}