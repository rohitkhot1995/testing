function showLoader() {
    document.getElementById("loader").style.display = "block";
    document.getElementById("loader-wrapper").style.display = "block";
    document.getElementById("loader-wrapper").classList.add("animate-bottom");
    document.getElementById("playerManagementForm").style.display = "none";
    document.getElementsByClassName("menu-section")[0].style.display = "none";
    document.getElementsByClassName("navbar")[0].style.display = "none";
    document.getElementsByClassName("footer-section")[0].style.display = "none";
    //document.getElementById("myDiv").style.display = "block";
}
function hideLoader() {
    document.getElementById("loader").style.display = "none";
    document.getElementById("loader-wrapper").style.display = "none";
    document.getElementById("playerManagementForm").style.display = "block";
    document.getElementsByClassName("menu-section")[0].style.display = "block";
    document.getElementsByClassName("navbar")[0].style.display = "block";
    document.getElementsByClassName("footer-section")[0].style.display = "block";
    //document.getElementById("myDiv").style.display = "block";
}