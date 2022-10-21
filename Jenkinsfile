node {
  stage('SCM') {
    checkout scm
  }
  stage('Build + SonarQube analysis') {
    def sqScannerMsBuildHome = tool 'SonarScanner for MSBuild'
    withSonarQubeEnv('sonar') {
      bat "${sqScannerMsBuildHome}\\SonarQube.Scanner.MSBuild.exe begin /k:dotnet"
      bat 'MSBuild.exe /t:Rebuild'
      bat "${sqScannerMsBuildHome}\\SonarQube.Scanner.MSBuild.exe end"
    }
  }
}
