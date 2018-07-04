from base64 import b64decode
from flask import Flask, request
import cv2
import matplotlib
matplotlib.use('Agg')
from matplotlib import pyplot as plt

app = Flask(__name__)




@app.route('/value', methods=['POST'])
def post():
    data = request.stream
    inp = data.read()
    b64imgfile = open('img.jpg', 'wb')
    b64imgfile.write(b64decode(inp))
    b64imgfile.close()
    filename = r"img.jpg"
    img = cv2.imread(filename, 0)
    plt.figure(1)
    plt.hist(img.ravel(), 256, [0, 256])
    plt.savefig("C:/Projs/SimpleImageGallery/SimpleImageGallery/wwwroot/images/fig1.png")
    plt.close(1)
    plt.figure(2)
    plt.imshow(img, cmap='gray', interpolation='bicubic')
    plt.xticks([]), plt.yticks([])  # to hide tick values on X and Y axis
    plt.savefig("/Projs/SimpleImageGallery/SimpleImageGallery/wwwroot/images/fig2.png")
    plt.close(2)
    return ''


if __name__ == '__main__':
    app.run()