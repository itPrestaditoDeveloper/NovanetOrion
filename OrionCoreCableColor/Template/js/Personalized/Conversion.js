function str2bool(strvalue) {
    return (strvalue && typeof strvalue === 'string') ? (strvalue.toLowerCase() === 'true') : (strvalue === true);
}

function ConvertirDecimal(valor) {
    return accounting.formatNumber(valor, 2);//  valor.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
