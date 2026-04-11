/**
 * Format a date string into a local Vietnamese date string
 * @param {string|Date} date 
 * @param {boolean} includeTime 
 * @returns {string}
 */
export const formatDate = (date, includeTime = true) => {
  if (!date) return '';
  const d = new Date(date);
  const options = {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  };
  if (includeTime) {
    options.hour = '2-digit';
    options.minute = '2-digit';
  }
  return d.toLocaleDateString('vi-VN', options);
};

/**
 * Format a number into Vietnamese currency (VNĐ)
 * @param {number} price 
 * @returns {string}
 */
export const formatPrice = (price) => {
  if (price === undefined || price === null) return '0';
  return price.toLocaleString('vi-VN');
};

/**
 * Extract time from a date string (HH:mm)
 * @param {string|Date} date 
 * @returns {string}
 */
export const formatTime = (date) => {
  if (!date) return '';
  const d = new Date(date);
  return d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', hour12: false });
};
