pipeline {
    agent any

    environment {
        DOCKER_REGISTRY = "docker.io/${DOCKER_USERNAME}"
        BACKEND_IMAGE = "shoe-backend"
        FRONTEND_IMAGE = "shoe-frontend"
        SERVER_HOST = "52.64.231.178"
        SERVER_USER = "ubuntu"
        PROJECT_DIR = "/home/ubuntu/project"
    }

    stages {

        stage('Checkout Source') {
            steps {
                echo "Fetching source code..."
                checkout([$class: 'GitSCM',
                    branches: [[name: '*/main']],
                    userRemoteConfigs: [[
                        url: 'https://github.com/duongtien004/FinalDevOp.git',
                        credentialsId: 'github-pat'
                    ]]
                ])
            }
        }

        stage('Build & Push Backend (.NET 8)') {
            steps {
                dir('Shoe_stores') {
                    withCredentials([usernamePassword(credentialsId: 'dockerhub-cred', usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                        sh '''
                            echo "Building .NET backend image..."
                            docker build -t $DOCKER_USER/shoe-backend:latest .

                            echo "Logging into DockerHub..."
                            echo "$DOCKER_PASS" | docker login -u "$DOCKER_USER" --password-stdin

                            echo "Pushing backend image..."
                            docker push $DOCKER_USER/shoe-backend:latest
                        '''
                    }
                }
            }
        }

        stage('Build & Push Frontend (Vite)') {
            steps {
                dir('shoe-store-frontend') {
                    withCredentials([usernamePassword(credentialsId: 'dockerhub-cred', usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                        sh '''
                            echo "Building Vite frontend image..."
                            docker build -t $DOCKER_USER/shoe-frontend:latest .

                            echo "Logging into DockerHub..."
                            echo "$DOCKER_PASS" | docker login -u "$DOCKER_USER" --password-stdin

                            echo "Pushing frontend image..."
                            docker push $DOCKER_USER/shoe-frontend:latest
                        '''
                    }
                }
            }
        }

        stage('Deploy to Production Server') {
            steps {
                withCredentials([
                    usernamePassword(credentialsId: 'dockerhub-cred', usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')
                ]) {
                    sshagent(credentials: ['server-ssh-key']) {

                        sh '''
                        echo "Copying docker-compose.yml to server..."
                        scp -o StrictHostKeyChecking=no docker-compose.yml $SERVER_USER@$SERVER_HOST:$PROJECT_DIR/docker-compose.yml

                        echo "Deploying to $SERVER_HOST..."

                        ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_HOST << EOF
                        set -e
                        cd $PROJECT_DIR

                        echo "Logging into DockerHub..."
                        echo "$DOCKER_PASS" | docker login -u "$DOCKER_USER" --password-stdin

                        echo "Pulling new images..."
                        docker compose pull

                        echo "Restarting services..."
                        docker compose down || true
                        docker compose up -d

                        echo "Removing old images..."
                        docker image prune -f

                        echo "Deployment finished!"
EOF
                        '''
                    }
                }
            }
        }
    }

    post {
        success {
            echo "🚀 DEPLOY THÀNH CÔNG! Website chạy tại: http://${SERVER_HOST}:3000"
        }
        failure {
            echo "❌ DEPLOY FAILED – Kiểm tra log Jenkins!"
        }
        always {
            cleanWs()
        }
    }
}
