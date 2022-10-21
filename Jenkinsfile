node {
  stage('SCM') {
    checkout scm
  }
  stage('SonarQube Analysis') {
       
        def scannerHome = tool 'SonarScanner for MSBuild'
        withSonarQubeEnv('sonar') {
          
          sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"dotnet\""      
          sh "dotnet build"
          sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"           
        }

    }
}

