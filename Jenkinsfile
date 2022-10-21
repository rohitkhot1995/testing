node {
  stage('SCM') {
    checkout scm
  }
  stage('SonarQube Analysis') {
      updateGitlabCommitStatus name: 'SonarQube Analysis', state: 'pending'
       
        def scannerHome = tool 'SonarScanner for MSBuild'
        withSonarQubeEnv('sonar') {
          
          sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"dotnet\""      
          sh "dotnet build Gaming.Predictor.sln"
          sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"           
        }

      updateGitlabCommitStatus name: 'SonarQube Analysis', state: 'success'
    }
}

