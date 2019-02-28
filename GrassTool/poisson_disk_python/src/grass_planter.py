"""Grass Planter.

Usage:
  grass_planter.py --out=<out> --map=<map> --high_density_radius=<radius> --low_density_radius=<radius>
  grass_planter.py (-h | --help)
  grass_planter.py --version

Options:
  -h --help     Show this screen.
  --version     Show version.
"""

from docopt import docopt

from PIL import Image

from poisson_disk import *
from enhanced_grid import *


def convert_enhanced_grid_to_png(grid, fname):
    image = Image.new('RGBA', grid.dims)

    pix = image.load()

    for index in grid.index_iter():
        grey = int(255 * grid[index])
        pix[index] = (grey, grey, grey, 255)

    image.save(fname)


def place_grass(out_path, density_map_path, high_density_radius, low_density_radius):
    density_map = Image.open(density_map_path)
    width, height = density_map.size

    r_grid = Grid2D((width, height))

    for index in r_grid.index_iter():
        density = density_map.getpixel(index)[0] / 255.0
        r_grid[index] = high_density_radius + (1.0 - density) * (low_density_radius - high_density_radius)

    p = sample_poisson(width, height, r_grid, 30)
    g = points_to_grid(p, (width, height))
    convert_enhanced_grid_to_png(g, out_path)


if __name__ == '__main__':
    arguments = docopt(__doc__, version='Grass Planter 0.1')
    place_grass(arguments['--out'], arguments['--map'], int(arguments['--high_density_radius']), int(arguments['--low_density_radius']))
