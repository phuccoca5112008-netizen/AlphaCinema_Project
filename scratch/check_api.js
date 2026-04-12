
async function checkApi() {
  try {
    console.log('Logging in...');
    const loginRes = await fetch('http://localhost:5059/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Email: 'admin@alpha.com', MatKhau: 'admin123' })
    });
    
    const loginData = await loginRes.json();
    if (!loginData.success) {
        console.error('Login failed:', JSON.stringify(loginData, null, 2));
        return;
    }

    const token = loginData.data?.token || loginData.token;
    console.log('Token obtained.');

    const invoicesRes = await fetch('http://localhost:5059/api/hoa-don', {
      headers: { Authorization: `Bearer ${token}` }
    });

    const invoicesData = await invoicesRes.json();
    console.log('Invoices Data (First item):');
    console.log(JSON.stringify(invoicesData.data[0], null, 2));
  } catch (error) {
    console.error('Error:', error.message);
  }
}

checkApi();
