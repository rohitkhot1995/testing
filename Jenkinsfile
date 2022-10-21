node {
  stage('SCM') {
    check SCM
  }
  stage('Build + SonarQube analysis') {
    def sqScannerMsBuildHome = tool 'Scanner for MSBuild'
    withSonarQubeEnv('My SonarQube Server') {
      bat "${sqScannerMsBuildHome}\\SonarQube.Scanner.MSBuild.exe begin /k:myKey"
      bat 'MSBuild.exe /t:Rebuild'
      bat "${sqScannerMsBuildHome}\\SonarQube.Scanner.MSBuild.exe end"
    }
  }
}
