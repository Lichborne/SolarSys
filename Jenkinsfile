pipeline {
    agent {
        docker { image 'unityci/editor:ubuntu-2020.3.25f1-webgl-0.16.1' 
            alwaysPull false
        }
    }
    stages {
        stage('Test') {
            steps {
                sh 'ls -la'
            }
        }
    }
}
