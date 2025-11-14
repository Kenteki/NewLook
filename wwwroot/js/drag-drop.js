// Drag and Drop functionality for custom fields and ID elements

window.dragDropHelper = {
    initializeDragDrop: function (containerId, dotNetHelper) {
        const container = document.getElementById(containerId);
        if (!container) return;

        const items = container.querySelectorAll('.draggable-item');

        items.forEach((item, index) => {
            item.setAttribute('draggable', 'true');
            item.style.cursor = 'move';

            item.addEventListener('dragstart', function (e) {
                e.dataTransfer.effectAllowed = 'move';
                e.dataTransfer.setData('text/html', this.innerHTML);
                this.classList.add('dragging');
                e.dataTransfer.setData('oldIndex', index);
            });

            item.addEventListener('dragend', function (e) {
                this.classList.remove('dragging');
                items.forEach(item => item.classList.remove('drag-over'));
            });

            item.addEventListener('dragover', function (e) {
                e.preventDefault();
                e.dataTransfer.dropEffect = 'move';

                const dragging = container.querySelector('.dragging');
                if (dragging && dragging !== this) {
                    this.classList.add('drag-over');
                }
                return false;
            });

            item.addEventListener('dragleave', function (e) {
                this.classList.remove('drag-over');
            });

            item.addEventListener('drop', function (e) {
                e.stopPropagation();
                e.preventDefault();

                const dragging = container.querySelector('.dragging');
                if (dragging && dragging !== this) {
                    const allItems = Array.from(container.querySelectorAll('.draggable-item'));
                    const oldIndex = allItems.indexOf(dragging);
                    const newIndex = allItems.indexOf(this);

                    if (oldIndex < newIndex) {
                        this.parentNode.insertBefore(dragging, this.nextSibling);
                    } else {
                        this.parentNode.insertBefore(dragging, this);
                    }

                    // Notify Blazor component
                    if (dotNetHelper) {
                        dotNetHelper.invokeMethodAsync('OnItemReordered', oldIndex, newIndex);
                    }
                }

                this.classList.remove('drag-over');
                return false;
            });
        });
    },

    cleanup: function (containerId) {
        const container = document.getElementById(containerId);
        if (!container) return;

        const items = container.querySelectorAll('.draggable-item');
        items.forEach(item => {
            item.removeAttribute('draggable');
            item.style.cursor = '';
            item.classList.remove('dragging', 'drag-over');
        });
    }
};
